namespace RawCef.Native;

public unsafe partial struct _cef_domvisitor_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void (*)(struct _cef_domvisitor_t *, struct _cef_domdocument_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_domvisitor_t*, _cef_domdocument_t*, void> visit;
}
