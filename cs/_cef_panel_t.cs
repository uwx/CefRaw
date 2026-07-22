namespace RawCef.Native;

public partial struct _cef_panel_t
{
}

public unsafe partial struct _cef_panel_t
{
    [NativeTypeName("cef_view_t")]
    public _cef_view_t @base;

    [NativeTypeName("struct _cef_window_t *(*)(struct _cef_panel_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_window_t*> as_window;

    [NativeTypeName("struct _cef_fill_layout_t *(*)(struct _cef_panel_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_fill_layout_t*> set_to_fill_layout;

    [NativeTypeName("struct _cef_box_layout_t *(*)(struct _cef_panel_t *, const cef_box_layout_settings_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_box_layout_settings_t*, _cef_box_layout_t*> set_to_box_layout;

    [NativeTypeName("struct _cef_layout_t *(*)(struct _cef_panel_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_layout_t*> get_layout;

    [NativeTypeName("void (*)(struct _cef_panel_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, void> layout;

    [NativeTypeName("void (*)(struct _cef_panel_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_view_t*, void> add_child_view;

    [NativeTypeName("void (*)(struct _cef_panel_t *, struct _cef_view_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_view_t*, int, void> add_child_view_at;

    [NativeTypeName("void (*)(struct _cef_panel_t *, struct _cef_view_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_view_t*, int, void> reorder_child_view;

    [NativeTypeName("void (*)(struct _cef_panel_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, _cef_view_t*, void> remove_child_view;

    [NativeTypeName("void (*)(struct _cef_panel_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, void> remove_all_child_views;

    [NativeTypeName("size_t (*)(struct _cef_panel_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, nuint> get_child_view_count;

    [NativeTypeName("struct _cef_view_t *(*)(struct _cef_panel_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_panel_t*, int, _cef_view_t*> get_child_view_at;
}
