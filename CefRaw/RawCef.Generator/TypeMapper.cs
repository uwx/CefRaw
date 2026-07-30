using System.Text.RegularExpressions;

namespace CefRaw.SourceGen;

/// <summary>
/// Maps native CEF types to managed types and produces marshaling code snippets.
/// All mapping rules are hardcoded here — no configuration needed.
/// </summary>
internal static partial class TypeMapper
{
    // Regex to detect CEF enum names: cef_*_t (but not _cef_*_t)
    [GeneratedRegex(@"^cef_\w+_t$", RegexOptions.Compiled)]
    private static partial Regex CefEnumRegex { get; }

    // Regex to detect CEF struct names: _cef_*_t
    [GeneratedRegex(@"^_cef_\w+_t$", RegexOptions.Compiled)]
    private static partial Regex CefStructRegex { get; }

    /// <summary>
    /// Classifies a parameter in a CEF function-pointer signature.
    /// </summary>
    public enum ParamKind
    {
        /// <summary>Normal input — passed through as-is.</summary>
        Input,

        /// <summary>Input string — <c>const cef_string_t*</c> → <c>string?</c>.</summary>
        InputString,

        /// <summary>Output string — <c>cef_string_t*</c> (non-const) → writable by CEF.</summary>
        OutputString,

        /// <summary>Input array — <c>T* const*</c> → <c>ReadOnlySpan&lt;ICefT?&gt;</c>.</summary>
        InputArray,

        /// <summary>Output CEF object — <c>T**</c> (non-const) → <c>out ICefT?</c>.</summary>
        Output,
    }

    // Regex to extract the parameter list from a C function-pointer type:
    //   return_type (*)(params) [__attribute__((cc))]
    // Uses non-greedy match so it stops at the first ')' (param list end),
    // working with or without the trailing __attribute__.
    [GeneratedRegex(@"\(\*\)\((.+?)\)")]
    private static partial Regex NativeParamListRegex { get; }

    /// <summary>
    /// Parses the native C type string (which preserves <c>const</c>) and returns
    /// a <see cref="ParamKind"/> for every parameter position, including <c>self</c> at index 0.
    /// The count matches the C# delegate parameter count.
    /// </summary>
    public static List<ParamKind> ClassifyParams(string nativeCType)
    {
        var kinds = new List<ParamKind>();

        var m = NativeParamListRegex.Match(nativeCType);
        if (!m.Success) return kinds;

        var paramList = m.Groups[1].Value;
        // Split by ", " (the CEF translator uses a single space after comma)
        var @params = paramList.Split(", ");

        foreach (var p in @params)
        {
            var trimmed = p.Trim();
            kinds.Add(ClassifySingleParam(trimmed));
        }

        return kinds;
    }

    private static ParamKind ClassifySingleParam(string nativeParamType)
    {
        // void** is a generic write-back pointer, not a CEF object output
        if (nativeParamType.Contains("void"))
            return ParamKind.Input;

        // T* const* or T*const* or T*const * → InputArray (only for CEF struct types)
        if (nativeParamType.Contains("*const") && nativeParamType.Contains("_cef_"))
            return ParamKind.InputArray;

        // T** (no const between stars) → Output
        var starCount = nativeParamType.Count(c => c == '*');
        if (starCount >= 2 && !nativeParamType.Contains("const"))
            return ParamKind.Output;

        // Check if this is a CEF string type (exact match, not _cef_string_visitor_t etc.)
        var isCefStringType = nativeParamType.Contains("cef_string_t")    // cef_string_t
            || nativeParamType.Contains("_cef_string_utf16_t")           // _cef_string_utf16_t
            || nativeParamType.Contains("_cef_string_utf8_t")            // _cef_string_utf8_t
            || nativeParamType.Contains("_cef_string_wide_t");           // _cef_string_wide_t

        // cef_string_t* (non-const, single pointer) → OutputString
        if (starCount == 1 && !nativeParamType.Contains("const") && isCefStringType)
            return ParamKind.OutputString;

        // const cef_string_t* → InputString
        if (starCount == 1 && nativeParamType.Contains("const") && isCefStringType)
            return ParamKind.InputString;

        // Everything else → Input
        return ParamKind.Input;
    }

    /// <summary>
    /// Compute the managed wrapper class name from a native struct name.
    /// "_cef_response_t" → "CefResponse"
    /// "_cef_browser_host_t" → "CefBrowserHost"
    /// </summary>
    public static string GetManagedName(string nativeStructName)
    {
        var inner = nativeStructName;
        if (inner.StartsWith("_cef_") && inner.EndsWith("_t"))
            inner = inner.Substring("_cef_".Length, inner.Length - "_cef_".Length - "_t".Length);
        else if (inner.StartsWith("cef_") && inner.EndsWith("_t"))
            inner = inner.Substring("cef_".Length, inner.Length - "cef_".Length - "_t".Length);

        return "Cef" + SnakeToPascal(inner);
    }

    /// <summary>
    /// Map a native type name to its managed equivalent for use in method signatures.
    /// Returns the managed type string and marshaling hints via out params.
    /// </summary>
    public static string MapNativeToManaged(
        string nativeType,
        out bool isString,
        out bool isCefObject,
        out string? cefObjectName)
    {
        isString = false;
        isCefObject = false;
        cefObjectName = null;

        // Strip pointer suffix for analysis
        var baseType = nativeType.TrimEnd('*', ' ');
        var isPointer = nativeType.Contains("*");

        // Primitives pass through
        if (baseType is "int" or "void" or "double" or "float" or "long" or "ulong"
            or "nuint" or "nint" or "byte" or "sbyte" or "short" or "ushort" or "uint"
            or "bool" or "size_t")
        {
            // If it was a pointer to a primitive, keep it as-is
            return nativeType;
        }

        // Handle special size_t → nuint
        if (baseType == "size_t")
            return isPointer ? "nuint" : "nuint";

        // _cef_string_utf16_t (embedded by value) used as a field — handled differently
        if (baseType == "_cef_string_utf16_t" && !isPointer)
        {
            // This is a value-type string field, use CefString in data struct wrappers
            // For function signatures this shouldn't appear without a pointer
            return "CefString";
        }

        // _cef_string_utf16_t* (pointer) — userfree return or const param
        if ((baseType == "_cef_string_utf16_t" && isPointer) || baseType == "cef_string_userfree_utf16_t")
        {
            isString = true;
            return "string?";
        }

        // _cef_string_utf8_t* / _cef_string_wide_t*
        if ((baseType == "_cef_string_utf8_t" || baseType == "_cef_string_wide_t" && isPointer) || baseType == "cef_string_userfree_utf8_t" || baseType == "cef_string_userfree_wide_t")
        {
            isString = true;
            return "string?";
        }

        // cef_string_multimap_t — keep raw for now (no struct definition in XML)
        if (baseType == "cef_string_multimap_t" || baseType == "_cef_string_multimap_t")
            return nativeType;

        // cef_string_map_t — keep raw for now (no struct definition in XML)
        if (baseType == "cef_string_map_t" || baseType == "_cef_string_map_t")
            return nativeType;

        // cef_string_userfree_t is an alias for _cef_string_utf16_t*
        if (baseType == "cef_string_userfree_t")
        {
            isString = true;
            return "string?";
        }

        // _cef_*_t* (CEF object pointer)
        if (isPointer && CefStructRegex.IsMatch(baseType))
        {
            isCefObject = true;
            cefObjectName = GetManagedName(baseType);
            return "I" + cefObjectName + "?";
        }

        // cef_*_t (CEF enum, no pointer) — pass through as native type name for now
        if (!isPointer && CefEnumRegex.IsMatch(baseType))
        {
            return baseType;
        }

        // Fallback: pass through as-is
        return nativeType;
    }

    /// <summary>
    /// Determine if a field is a string value type (embedded _cef_string_utf16_t).
    /// </summary>
    public static bool IsStringValueField(string csharpType, string? nativeTypeName)
    {
        return csharpType == "_cef_string_utf16_t" ||
               nativeTypeName != null && (nativeTypeName.Contains("cef_string_t") && !nativeTypeName.Contains("*"));
    }

    /// <summary>
    /// Determine if a type is a CEF enum type (cef_*_t, no pointer).
    /// </summary>
    public static bool IsCefEnum(string csharpType)
    {
        return CefEnumRegex.IsMatch(csharpType);
    }

    /// <summary>
    /// "browser_subprocess_path" → "BrowserSubprocessPath"
    /// </summary>
    public static string SnakeToPascal(string snake)
    {
        var parts = snake.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
                parts[i] = char.ToUpperInvariant(parts[i][0]) + parts[i].Substring(1);
        }
        return string.Concat(parts);
    }

    [GeneratedRegex(@"delegate\* unmanaged\[(?:Cdecl|Stdcall)\]<((?:[\w_*]+, )+)([\w_*]+)>")]
    private static partial Regex DelegateRegex { get; }

    public static (string[] Args, string Return) GetTypeListFromDelegate(string delegateTypeName)
    {
        // delegate* unmanaged[Cdecl]<ushort*, void>
        var match = DelegateRegex.Match(delegateTypeName);
        if (!match.Success) throw new InvalidOperationException("Input is not unmanaged delegate");

        var args = match.Groups[1].Value.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var @return = match.Groups[2].Value;

        return (args, @return);
    }

    public static string CefFunctionToCSharp(string functionName)
    {
        return SnakeToPascal(functionName).Replace("Cef", "");
    }
}
