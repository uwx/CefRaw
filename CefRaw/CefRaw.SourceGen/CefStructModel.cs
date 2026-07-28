using System.Collections.Generic;

namespace CefRaw.SourceGen;

/// <summary>
/// Extracted model of a CEF native struct that needs a managed wrapper.
/// </summary>
internal sealed class CefStructModel
{
    /// <summary>Native struct name, e.g. "_cef_response_t".</summary>
    public string NativeName { get; set; } = string.Empty;

    /// <summary>Managed wrapper name, e.g. "CefResponse".</summary>
    public string ManagedName { get; set; } = string.Empty;

    /// <summary>Base type classification.</summary>
    public CefBaseType BaseType { get; set; }

    /// <summary>True if the struct has at least one function-pointer field.</summary>
    public bool IsCefObject { get; set; }

    /// <summary>Data fields (non-function-pointer), for data-only structs.</summary>
    public List<CefFieldModel> DataFields { get; set; } = new();

    /// <summary>Function-pointer fields that become methods in the wrapper.</summary>
    public List<CefMethodModel> Methods { get; set; } = new();

    /// <summary>Combined name segments for PascalCase conversion.</summary>
    public string NativeNameWithoutPrefix
    {
        get
        {
            var name = NativeName;
            if (name.StartsWith("_cef_") && name.EndsWith("_t"))
                return name.Substring("_cef_".Length, name.Length - "_cef_".Length - "_t".Length);
            return name;
        }
    }
}

internal enum CefBaseType
{
    None,
    RefCounted,
    Scoped
}

/// <summary>
/// A data field on a native struct (not a function pointer).
/// </summary>
internal sealed class CefFieldModel
{
    public string Name { get; set; } = string.Empty;
    public string NativeType { get; set; } = string.Empty;
    public string NativeTypeName { get; set; } = string.Empty; // from [NativeTypeName] attribute
    public bool IsStringValue { get; set; } // _cef_string_utf16_t embedded by value
    public bool IsEnumType { get; set; }   // cef_*_t without pointer
}

/// <summary>
/// A function-pointer field that becomes a method in the managed wrapper.
/// </summary>
internal sealed class CefMethodModel
{
    /// <summary>Field name, e.g. "is_read_only".</summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>PascalCase method name, e.g. "IsReadOnly".</summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>Native return type, e.g. "int", "void", "_cef_string_utf16_t*".</summary>
    public string NativeReturnType { get; set; } = string.Empty;

    /// <summary>Managed return type, e.g. "int", "string?".</summary>
    public string ManagedReturnType { get; set; } = string.Empty;

    /// <summary>Parameters (excluding the self-pointer first parameter).</summary>
    public List<CefParamModel> Parameters { get; set; } = new();

    /// <summary>True if the native return type is a CEF string pointer that needs freeing.</summary>
    public bool ReturnsUserFreeString { get; set; }

    /// <summary>True if the native return type is a CEF object pointer that needs wrapping.</summary>
    public bool ReturnsCefObject { get; set; }

    /// <summary>If ReturnsCefObject, the managed wrapper name (e.g. "CefBrowserHost").</summary>
    public string ReturnCefObjectName { get; set; } = string.Empty;

    /// <summary>The NativeTypeName attribute value for this field, for debugging/hints.</summary>
    public string NativeTypeName { get; set; } = string.Empty;
}

/// <summary>
/// A parameter on a native function pointer.
/// </summary>
internal sealed class CefParamModel
{
    public int Index { get; set; }
    public string NativeType { get; set; } = string.Empty;
    public string ManagedType { get; set; } = string.Empty;
    public string ParamName { get; set; } = string.Empty; // e.g. "arg0", "str", "obj"
    public bool IsStringParam { get; set; }
    public bool IsCefObjectParam { get; set; }
    public string CefObjectName { get; set; } = string.Empty; // if IsCefObjectParam
}
