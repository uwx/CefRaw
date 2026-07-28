namespace RawCef.Native;

public unsafe partial struct _cef_menu_button_t
{
    [NativeTypeName("cef_label_button_t")]
    public _cef_label_button_t @base;

    [NativeTypeName("void (*)(struct _cef_menu_button_t *, struct _cef_menu_model_t *, const cef_point_t *, cef_menu_anchor_position_t) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_menu_button_t*, _cef_menu_model_t*, _cef_point_t*, cef_menu_anchor_position_t, void> show_menu;

    [NativeTypeName("void (*)(struct _cef_menu_button_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_menu_button_t*, void> trigger_menu;
}

public partial struct _cef_menu_button_t
{
}

public partial struct _cef_menu_button_t
{
}
