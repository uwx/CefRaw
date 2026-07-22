namespace RawCef.Native;

public unsafe partial struct _cef_button_delegate_t
{
    [NativeTypeName("cef_view_delegate_t")]
    public _cef_view_delegate_t @base;

    [NativeTypeName("void (*)(struct _cef_button_delegate_t *, struct _cef_button_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_button_delegate_t*, _cef_button_t*, void> on_button_pressed;

    [NativeTypeName("void (*)(struct _cef_button_delegate_t *, struct _cef_button_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_button_delegate_t*, _cef_button_t*, void> on_button_state_changed;
}
