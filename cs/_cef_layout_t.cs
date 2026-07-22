namespace RawCef.Native;

public partial struct _cef_layout_t
{
}

public unsafe partial struct _cef_layout_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("struct _cef_box_layout_t *(*)(struct _cef_layout_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_layout_t*, _cef_box_layout_t*> as_box_layout;

    [NativeTypeName("struct _cef_fill_layout_t *(*)(struct _cef_layout_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_layout_t*, _cef_fill_layout_t*> as_fill_layout;

    [NativeTypeName("int (*)(struct _cef_layout_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_layout_t*, int> is_valid;
}
