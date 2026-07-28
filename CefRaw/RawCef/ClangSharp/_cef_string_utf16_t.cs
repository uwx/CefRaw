namespace RawCef.Native;

public unsafe partial struct _cef_string_utf16_t
{
    [NativeTypeName("char16_t *")]
    public ushort* str;

    [NativeTypeName("size_t")]
    public nuint length;

    [NativeTypeName("void (*)(char16_t *)")]
    public delegate* unmanaged[Cdecl]<ushort*, void> dtor;
}
