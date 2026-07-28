namespace RawCef.Native;

public unsafe partial struct _cef_view_delegate_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("cef_size_t (*)(struct _cef_view_delegate_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, _cef_size_t> get_preferred_size;

    [NativeTypeName("cef_size_t (*)(struct _cef_view_delegate_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, _cef_size_t> get_minimum_size;

    [NativeTypeName("cef_size_t (*)(struct _cef_view_delegate_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, _cef_size_t> get_maximum_size;

    [NativeTypeName("int (*)(struct _cef_view_delegate_t *, struct _cef_view_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, int, int> get_height_for_width;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *, int, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, int, _cef_view_t*, void> on_parent_view_changed;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *, int, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, int, _cef_view_t*, void> on_child_view_changed;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, int, void> on_window_changed;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *, const cef_rect_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, _cef_rect_t*, void> on_layout_changed;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, void> on_focus;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, void> on_blur;

    [NativeTypeName("void (*)(struct _cef_view_delegate_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_view_delegate_t*, _cef_view_t*, void> on_theme_changed;
}
