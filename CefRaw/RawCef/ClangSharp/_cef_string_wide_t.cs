namespace RawCef.Native;

public unsafe partial struct _cef_string_wide_t
{
    [NativeTypeName("wchar_t *")]
    public uint* str;

    [NativeTypeName("size_t")]
    public nuint length;

    [NativeTypeName("void (*)(wchar_t *)")]
    public delegate* unmanaged[Cdecl]<uint*, void> dtor;
}
