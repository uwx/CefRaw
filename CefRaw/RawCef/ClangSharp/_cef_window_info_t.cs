namespace RawCef.Native;

public unsafe partial struct _cef_window_info_t
{
    [NativeTypeName("size_t")]
    public nuint size;

    [NativeTypeName("unsigned long")]
    public uint ex_style;

    [NativeTypeName("cef_string_t")]
    public _cef_string_utf16_t window_name;

    [NativeTypeName("unsigned long")]
    public uint style;

    [NativeTypeName("cef_rect_t")]
    public _cef_rect_t bounds;

    [NativeTypeName("cef_window_handle_t")]
    public void* parent_window;

    public void* menu;

    public int windowless_rendering_enabled;

    public int shared_texture_enabled;

    public int external_begin_frame_enabled;

    [NativeTypeName("cef_window_handle_t")]
    public void* window;

    public cef_runtime_style_t runtime_style;
}
