using RawCef.Native;

namespace RawCef;

/// <summary>
/// Represents a library-owned non-userfree CEF string.
/// </summary>
public unsafe class CefString
{
    private readonly _cef_string_utf16_t* _ptr;

    public string? Value
    {
        get
        {
            var result = _ptr->str is not null
                ? new string((char*)_ptr->str, 0, (int)_ptr->length)
                : string.Empty;

            return result;
        }
        set
        {
            fixed (char* str = value)
            {
                CefUnsafe.StringUtf16Set((ushort*)str, (nuint)(value?.Length ?? 0), _ptr, 1);
            }
        }
    }
    
    public CefString(_cef_string_utf16_t* str)
    {
        _ptr = str;
    }
}