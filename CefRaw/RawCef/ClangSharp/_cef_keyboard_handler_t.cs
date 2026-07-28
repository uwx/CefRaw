namespace RawCef.Native;

public unsafe partial struct _cef_keyboard_handler_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("int (*)(struct _cef_keyboard_handler_t *, struct _cef_browser_t *, const cef_key_event_t *, cef_event_handle_t, int *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_keyboard_handler_t*, _cef_browser_t*, _cef_key_event_t*, tagMSG*, int*, int> on_pre_key_event;

    [NativeTypeName("int (*)(struct _cef_keyboard_handler_t *, struct _cef_browser_t *, const cef_key_event_t *, cef_event_handle_t) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_keyboard_handler_t*, _cef_browser_t*, _cef_key_event_t*, tagMSG*, int> on_key_event;
}
