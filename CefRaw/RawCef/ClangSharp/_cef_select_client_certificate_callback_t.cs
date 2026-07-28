namespace RawCef.Native;

public unsafe partial struct _cef_select_client_certificate_callback_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void (*)(struct _cef_select_client_certificate_callback_t *, struct _cef_x509_certificate_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_select_client_certificate_callback_t*, _cef_x509_certificate_t*, void> select;
}
