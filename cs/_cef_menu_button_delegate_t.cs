namespace RawCef.Native;

public unsafe partial struct _cef_menu_button_delegate_t
{
    [NativeTypeName("cef_button_delegate_t")]
    public _cef_button_delegate_t @base;

    [NativeTypeName("void (*)(struct _cef_menu_button_delegate_t *, struct _cef_menu_button_t *, const cef_point_t *, struct _cef_menu_button_pressed_lock_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_menu_button_delegate_t*, _cef_menu_button_t*, _cef_point_t*, _cef_menu_button_pressed_lock_t*, void> on_menu_button_pressed;
}
