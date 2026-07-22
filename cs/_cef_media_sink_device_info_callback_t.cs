namespace RawCef.Native;

public unsafe partial struct _cef_media_sink_device_info_callback_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void (*)(struct _cef_media_sink_device_info_callback_t *, const struct _cef_media_sink_device_info_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_media_sink_device_info_callback_t*, _cef_media_sink_device_info_t*, void> on_media_sink_device_info;
}
