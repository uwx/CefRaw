namespace RawCef.Native;

public unsafe partial struct _cef_accelerated_paint_info_t
{
    [NativeTypeName("size_t")]
    public nuint size;

    [NativeTypeName("cef_shared_texture_handle_t")]
    public void* shared_texture_handle;

    public cef_color_type_t format;

    [NativeTypeName("cef_accelerated_paint_info_common_t")]
    public _cef_accelerated_paint_info_common_t extra;
}
