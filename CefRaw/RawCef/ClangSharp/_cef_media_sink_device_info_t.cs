namespace RawCef.Native;

public partial struct _cef_media_sink_device_info_t
{
    [NativeTypeName("size_t")]
    public nuint size;

    [NativeTypeName("cef_string_t")]
    public _cef_string_utf16_t ip_address;

    public int port;

    [NativeTypeName("cef_string_t")]
    public _cef_string_utf16_t model_name;
}
