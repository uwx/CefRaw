using RawCef.Native;

namespace RawCef;

/// <summary>
/// Implementation of <see cref="ICefStringList"/> methods wrapping the CEF string list API.
/// </summary>
public unsafe partial class CefStringListRef : IDisposable
{
    private int _disposed;

    /// <summary>
    /// Creates a new empty string list. The caller is responsible for disposal.
    /// </summary>
    public CefStringListRef() : this(CefUnsafe.StringListAlloc())
    {
    }

    /// <inheritdoc />
    public int Count => (int)CefUnsafe.StringListSize(_ptr);

    /// <inheritdoc />
    public string? GetValue(int index)
    {
        _cef_string_utf16_t str = default;
        if (CefUnsafe.StringListValue(_ptr, (nuint)index, &str) == 0)
            return null;
        return CefStringRef.ToStringAndFree(&str);
    }

    /// <inheritdoc />
    public void Append(string? value)
    {
        fixed (char* p = value)
        {
            _cef_string_utf16_t str;
            CefStringRef.FillFromPinned(&str, p, value?.Length ?? 0);
            CefUnsafe.StringListAppend(_ptr, &str);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        CefUnsafe.StringListClear(_ptr);
    }

    /// <inheritdoc />
    public ICefStringList Copy()
    {
        var copy = CefUnsafe.StringListCopy(_ptr);
        return new CefStringListRef(copy);
    }

    /// <summary>
    /// Frees the native string list.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        if (_ptr is not null)
        {
            CefUnsafe.StringListFree(_ptr);
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer as safety net.
    /// </summary>
    ~CefStringListRef()
    {
        Dispose();
    }
}
