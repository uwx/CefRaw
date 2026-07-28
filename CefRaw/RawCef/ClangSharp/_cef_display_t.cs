namespace RawCef.Native;

public unsafe partial struct _cef_display_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("int64_t (*)(struct _cef_display_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, long> get_id;

    [NativeTypeName("float (*)(struct _cef_display_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, float> get_device_scale_factor;

    [NativeTypeName("void (*)(struct _cef_display_t *, cef_point_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, _cef_point_t*, void> convert_point_to_pixels;

    [NativeTypeName("void (*)(struct _cef_display_t *, cef_point_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, _cef_point_t*, void> convert_point_from_pixels;

    [NativeTypeName("cef_rect_t (*)(struct _cef_display_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, _cef_rect_t> get_bounds;

    [NativeTypeName("cef_rect_t (*)(struct _cef_display_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, _cef_rect_t> get_work_area;

    [NativeTypeName("int (*)(struct _cef_display_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_display_t*, int> get_rotation;
}
