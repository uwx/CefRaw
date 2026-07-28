using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CefRaw.SourceGen;

[Generator]
public sealed class CefWrapperGenerator : IIncrementalGenerator
{
    private static readonly Regex s_structNameRegex = new(
        @"^_cef_\w+_t$", RegexOptions.Compiled);

    // Structs with hand-written wrappers (excluded from generation)
    private static readonly HashSet<string> s_excludedStructs = new()
    {
        "_cef_base_ref_counted_t",
        "_cef_base_scoped_t",
        // Platform-specific structs — different layouts per OS, hand-written with #if guards
        "_cef_main_args_t",
        "_cef_window_info_t",
        "_cef_accelerated_paint_info_t",
        "_cef_accelerated_paint_native_pixmap_plane_info_t",
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all _cef_*_t struct declarations
        var structDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => IsCefStruct(node),
                transform: (ctx, _) => GetStructTarget(ctx))
            .Where(target => target is not null)!;

        // Combine with compilation
        var compilationAndStructs = context.CompilationProvider.Combine(structDeclarations.Collect());

        // Register source output
        context.RegisterSourceOutput(compilationAndStructs, (spc, source) =>
        {
            var compilation = source.Left;
            var structTargets = source.Right;

            if (structTargets.IsEmpty)
                return;

            // Deduplicate by struct name (may have multiple partial declarations)
            var seenNames = new HashSet<string>();
            foreach (var target in structTargets)
            {
                var name = target.Identifier.Text;
                if (!seenNames.Add(name))
                    continue;

                var model = ExtractStructModel(compilation, target);
                if (model is not null)
                {
                    var sourceText = Emitter.GenerateWrapper(model);
                    spc.AddSource($"{model.ManagedName}.g.cs", sourceText);
                }
            }
        });
    }

    private static bool IsCefStruct(SyntaxNode node)
    {
        if (node is not StructDeclarationSyntax structDecl)
            return false;

        var name = structDecl.Identifier.Text;
        if (!s_structNameRegex.IsMatch(name))
            return false;

        // Skip excluded structs (hand-written wrappers)
        if (s_excludedStructs.Contains(name))
            return false;

        // Skip empty partial declarations (e.g. "public partial struct _cef_value_t { }")
        // Only keep the one that has actual members
        return structDecl.Members.Count > 0;
    }

    private static StructDeclarationSyntax? GetStructTarget(GeneratorSyntaxContext ctx)
    {
        var structDecl = (StructDeclarationSyntax)ctx.Node;

        // Must be in RawCef.Native namespace
        var ns = GetNamespace(structDecl);
        if (ns != "RawCef.Native")
            return null;

        // Must be partial
        if (!structDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            return null;

        return structDecl;
    }

    private static string? GetNamespace(SyntaxNode node)
    {
        var parent = node.Parent;
        while (parent != null)
        {
            if (parent is NamespaceDeclarationSyntax ns)
                return ns.Name.ToString();
            if (parent is FileScopedNamespaceDeclarationSyntax fns)
                return fns.Name.ToString();
            parent = parent.Parent;
        }
        return null;
    }

    private static CefStructModel? ExtractStructModel(
        Compilation compilation, StructDeclarationSyntax structDecl)
    {
        var semanticModel = compilation.GetSemanticModel(structDecl.SyntaxTree);
        var structSymbol = semanticModel.GetDeclaredSymbol(structDecl) as INamedTypeSymbol;
        if (structSymbol == null)
            return null;

        var nativeName = structSymbol.Name;
        var managedName = TypeMapper.GetManagedName(nativeName);

        var model = new CefStructModel
        {
            NativeName = nativeName,
            ManagedName = managedName,
        };

        // Inspect members
        var hasFunctionPointers = false;
        CefBaseType baseType = CefBaseType.None;

        foreach (var member in structSymbol.GetMembers())
        {
            if (member is not IFieldSymbol field)
                continue;

            // Check if this is the @base field
            if (field.Name == "base")
            {
                baseType = field.Type.Name switch
                {
                    "_cef_base_ref_counted_t" => CefBaseType.RefCounted,
                    "_cef_base_scoped_t" => CefBaseType.Scoped,
                    _ => CefBaseType.None
                };
                continue;
            }

            // Skip the 'del' field in scoped base (it's a destructor, not a method)
            if (field.Name == "del" && baseType == CefBaseType.Scoped)
                continue;

            // Check if it's a function pointer type
            if (field.Type.TypeKind == TypeKind.FunctionPointer)
            {
                hasFunctionPointers = true;
                var method = ExtractMethodModel(field, nativeName);
                if (method is not null)
                    model.Methods.Add(method);
            }
            else if (field.Type.TypeKind == TypeKind.Pointer &&
                     ((IPointerTypeSymbol)field.Type).PointedAtType.TypeKind == TypeKind.FunctionPointer)
            {
                // delegate* wrapped in a pointer (shouldn't happen but handle it)
                hasFunctionPointers = true;
            }
            else
            {
                // Data field
                var nativeTypeName = GetNativeTypeName(field);
                var fieldModel = new CefFieldModel
                {
                    Name = field.Name,
                    NativeType = field.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    NativeTypeName = nativeTypeName ?? string.Empty,
                    IsStringValue = TypeMapper.IsStringValueField(
                        field.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                        nativeTypeName ?? string.Empty),
                    IsEnumType = TypeMapper.IsCefEnum(
                        field.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)),
                };
                model.DataFields.Add(fieldModel);
            }
        }

        model.BaseType = baseType;
        model.IsCefObject = hasFunctionPointers;

        return model;
    }

    private static CefMethodModel? ExtractMethodModel(IFieldSymbol field, string structName)
    {
        var fpType = field.Type as IFunctionPointerTypeSymbol;
        if (fpType == null)
            return null;

        var signature = fpType.Signature;
        if (signature == null)
            return null;

        var parameters = signature.Parameters;
        // First parameter should be the self pointer (_cef_*_t*)
        // We skip it for the managed wrapper
        var skipCount = parameters.Length > 0 &&
                        parameters[0].Type is IPointerTypeSymbol ptr &&
                        ptr.PointedAtType.Name == structName
            ? 1 : 0;

        var nativeReturnType = signature.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var nativeTypeName = GetNativeTypeName(field);

        var managedReturnType = TypeMapper.MapNativeToManaged(
            nativeReturnType, isReturnType: true,
            out var returnsString, out var returnsObject, out var returnObjectName);

        var method = new CefMethodModel
        {
            FieldName = field.Name,
            MethodName = TypeMapper.SnakeToPascal(field.Name),
            NativeReturnType = nativeReturnType,
            ManagedReturnType = managedReturnType,
            ReturnsUserFreeString = returnsString,
            ReturnsCefObject = returnsObject,
            ReturnCefObjectName = returnObjectName ?? string.Empty,
            NativeTypeName = nativeTypeName ?? string.Empty,
        };

        // Map parameters (skip self-pointer)
        int paramIndex = 0;
        for (int i = skipCount; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var nativeParamType = param.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            var managedParamType = TypeMapper.MapNativeToManaged(
                nativeParamType, isReturnType: false,
                out var isString, out var isCefObj, out var cefObjName);

            method.Parameters.Add(new CefParamModel
            {
                Index = paramIndex,
                NativeType = nativeParamType,
                ManagedType = managedParamType,
                ParamName = "arg" + paramIndex,
                IsStringParam = isString,
                IsCefObjectParam = isCefObj,
                CefObjectName = cefObjName ?? string.Empty,
            });

            paramIndex++;
        }

        return method;
    }

    private static string? GetNativeTypeName(IFieldSymbol field)
    {
        foreach (var attr in field.GetAttributes())
        {
            if (attr.AttributeClass?.Name == "NativeTypeNameAttribute" &&
                attr.ConstructorArguments.Length > 0)
            {
                return attr.ConstructorArguments[0].Value?.ToString();
            }
        }
        return null;
    }
}
