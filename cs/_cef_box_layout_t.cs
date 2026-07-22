namespace RawCef.Native;

public partial struct _cef_box_layout_t
{
}

public partial struct _cef_box_layout_t
{
}

public unsafe partial struct _cef_box_layout_t
{
    [NativeTypeName("cef_layout_t")]
    public _cef_layout_t @base;

    [NativeTypeName("void (*)(struct _cef_box_layout_t *, struct _cef_view_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_box_layout_t*, _cef_view_t*, int, void> set_flex_for_view;

    [NativeTypeName("void (*)(struct _cef_box_layout_t *, struct _cef_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_box_layout_t*, _cef_view_t*, void> clear_flex_for_view;
}
