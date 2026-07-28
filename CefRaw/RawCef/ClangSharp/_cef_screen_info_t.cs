namespace RawCef.Native;

public partial struct _cef_screen_info_t
{
    [NativeTypeName("size_t")]
    public nuint size;

    public float device_scale_factor;

    public int depth;

    public int depth_per_component;

    public int is_monochrome;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t rect;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t available_rect;
}
