namespace RawCef.Native;

public unsafe partial struct _cef_preference_observer_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void (*)(struct _cef_preference_observer_t *, const cef_string_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_preference_observer_t*, _cef_string_utf16_t*, void> on_preference_changed;
}
