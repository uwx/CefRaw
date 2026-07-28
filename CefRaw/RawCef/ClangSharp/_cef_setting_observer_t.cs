namespace RawCef.Native;

public unsafe partial struct _cef_setting_observer_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void (*)(struct _cef_setting_observer_t *, const cef_string_t *, const cef_string_t *, cef_content_setting_types_t) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_setting_observer_t*, _cef_string_utf16_t*, _cef_string_utf16_t*, cef_content_setting_types_t, void> on_setting_changed;
}
