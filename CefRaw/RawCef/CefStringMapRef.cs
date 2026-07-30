using RawCef.Native;

namespace RawCef;

/// <summary>
/// Implementation of <see cref="ICefStringMap"/> methods wrapping the CEF string map API.
/// </summary>
public unsafe partial class CefStringMapRef : IDisposable
{
    private int _disposed;

    /// <summary>
    /// Creates a new empty string map. The caller is responsible for disposal.
    /// </summary>
    public CefStringMapRef() : this(CefUnsafe.StringMapAlloc())
    {
    }

    /// <inheritdoc />
    public int Count => (int)CefUnsafe.StringMapSize(_ptr);

    /// <inheritdoc />
    public string? Find(string key)
    {
        _cef_string_utf16_t keyStr = default;
        _cef_string_utf16_t valueStr = default;
        fixed (char* p = key)
        {
            CefStringRef.FillFromPinned(&keyStr, p, key.Length);
        }

        var found = CefUnsafe.StringMapFind(_ptr, &keyStr, &valueStr);
        if (found == 0)
            return null;
        return CefStringRef.ToStringAndFree(&valueStr);
    }

    /// <inheritdoc />
    public string? GetKey(int index)
    {
        _cef_string_utf16_t str = default;
        if (CefUnsafe.StringMapKey(_ptr, (nuint)index, &str) == 0)
            return null;
        return CefStringRef.ToStringAndFree(&str);
    }

    /// <inheritdoc />
    public string? GetValue(int index)
    {
        _cef_string_utf16_t str = default;
        if (CefUnsafe.StringMapValue(_ptr, (nuint)index, &str) == 0)
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
            CefUnsafe.StringMapAppend(_ptr, &keyStr, &valueStr);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        CefUnsafe.StringMapClear(_ptr);
    }

    /// <summary>
    /// Frees the native string map.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        if (_ptr is not null)
        {
            CefUnsafe.StringMapFree(_ptr);
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer as safety net.
    /// </summary>
    ~CefStringMapRef()
    {
        Dispose();
    }
}
