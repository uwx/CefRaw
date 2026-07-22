namespace RawCef.Native;

public unsafe partial struct _cef_textfield_delegate_t
{
    [NativeTypeName("cef_view_delegate_t")]
    public _cef_view_delegate_t @base;

    [NativeTypeName("int (*)(struct _cef_textfield_delegate_t *, struct _cef_textfield_t *, const cef_key_event_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_textfield_delegate_t*, _cef_textfield_t*, _cef_key_event_t*, int> on_key_event;

    [NativeTypeName("void (*)(struct _cef_textfield_delegate_t *, struct _cef_textfield_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_textfield_delegate_t*, _cef_textfield_t*, void> on_after_user_action;
}
