namespace RawCef.Native;

public partial struct _cef_scroll_view_t
{
}

public unsafe partial struct _cef_scroll_view_t
{
    [NativeTypeName("cef_view_t")]
    public _cef_view_t @base;

    [NativeTypeName("void (*)(struct _cef_scroll_view_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, _cef_view_t*, void> set_content_view;

    [NativeTypeName("struct _cef_view_t *(*)(struct _cef_scroll_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, _cef_view_t*> get_content_view;

    [NativeTypeName("cef_rect_t (*)(struct _cef_scroll_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, _cef_rect_t> get_visible_content_rect;

    [NativeTypeName("int (*)(struct _cef_scroll_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, int> has_horizontal_scrollbar;

    [NativeTypeName("int (*)(struct _cef_scroll_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, int> get_horizontal_scrollbar_height;

    [NativeTypeName("int (*)(struct _cef_scroll_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, int> has_vertical_scrollbar;

    [NativeTypeName("int (*)(struct _cef_scroll_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_scroll_view_t*, int> get_vertical_scrollbar_width;
}
