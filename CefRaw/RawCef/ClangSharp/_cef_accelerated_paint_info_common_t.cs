namespace RawCef.Native;

public partial struct _cef_accelerated_paint_info_common_t
{
    [NativeTypeName("size_t")]
    public nuint size;

    [NativeTypeName("uint64_t")]
    public ulong timestamp;

    [NativeTypeName("cef_size_t")]
    public _cef_size_t coded_size;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t visible_rect;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t content_rect;

    [NativeTypeName("cef_size_t")]
    public _cef_size_t source_size;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t capture_update_rect;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t region_capture_rect;

    [NativeTypeName("uint64_t")]
    public ulong capture_counter;

    [NativeTypeName("uint8_t")]
    public byte has_capture_update_rect;

    [NativeTypeName("uint8_t")]
    public byte has_region_capture_rect;

    [NativeTypeName("uint8_t")]
    public byte has_source_size;

    [NativeTypeName("uint8_t")]
    public byte has_capture_counter;
}
