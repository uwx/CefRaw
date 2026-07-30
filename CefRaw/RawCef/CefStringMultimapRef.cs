using RawCef.Native;

namespace RawCef;

/// <summary>
/// Implementation of <see cref="ICefStringMultimap"/> methods wrapping the CEF string multimap API.
/// </summary>
public unsafe partial class CefStringMultimapRef : IDisposable
{
    private int _disposed;

    /// <summary>
    /// Creates a new empty string multimap. The caller is responsible for disposal.
    /// </summary>
    public CefStringMultimapRef() : this(CefUnsafe.StringMultimapAlloc())
    {
    }

    /// <inheritdoc />
    public int Count => (int)CefUnsafe.StringMultimapSize(_ptr);

    /// <inheritdoc />
    public int FindCount(string key)
    {
        _cef_string_utf16_t keyStr = default;
        fixed (char* p = key)
        {
            CefStringRef.FillFromPinned(&keyStr, p, key.Length);
        }
        return (int)CefUnsafe.StringMultimapFindCount(_ptr, &keyStr);
    }

    /// <inheritdoc />
    public string? Enumerate(string key, int valueIndex)
    {
        _cef_string_utf16_t keyStr = default;
        _cef_string_utf16_t valueStr = default;
        fixed (char* p = key)
        {
            CefStringRef.FillFromPinned(&keyStr, p, key.Length);
        }

        if (CefUnsafe.StringMultimapEnumerate(_ptr, &keyStr, (nuint)valueIndex, &valueStr) == 0)
            return null;
        return CefStringRef.ToStringAndFree(&valueStr);
    }

    /// <inheritdoc />
    public string? GetKey(int index)
    {
        _cef_string_utf16_t str = default;
        if (CefUnsafe.StringMultimapKey(_ptr, (nuint)index, &str) == 0)
            return null;
        return CefStringRef.ToStringAndFree(&str);
    }

    /// <inheritdoc />
    public string? GetValue(int index)
    {
        _cef_string_utf16_t str = default;
        if (CefUnsafe.StringMultimapValue(_ptr, (nuint)index, &str) == 0)
            return null;
        return CefStringRef.ToStringAndFree(&str);
    }

    /// <inheritdoc />
    public void Append(string key, string value)
    {
        fixed (char* pk = key, pv = value)
        {
            _cef_string_utf16_t keyStr;
            _cef_string_utf16_t valueStr;
            CefStringRef.FillFromPinned(&keyStr, pk, key.Length);
            CefStringRef.FillFromPinned(&valueStr, pv, value.Length);
            CefUnsafe.StringMultimapAppend(_ptr, &keyStr, &valueStr);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        CefUnsafe.StringMultimapClear(_ptr);
    }

    /// <summary>
    /// Frees the native string multimap.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        if (_ptr is not null)
        {
            CefUnsafe.StringMultimapFree(_ptr);
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer as safety net.
    /// </summary>
    ~CefStringMultimapRef()
    {
        Dispose();
    }
}
