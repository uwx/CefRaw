namespace RawCef.Native;

public unsafe partial struct _cef_component_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("cef_string_userfree_t (*)(struct _cef_component_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_t*, _cef_string_utf16_t*> get_id;

    [NativeTypeName("cef_string_userfree_t (*)(struct _cef_component_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_t*, _cef_string_utf16_t*> get_name;

    [NativeTypeName("cef_string_userfree_t (*)(struct _cef_component_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_t*, _cef_string_utf16_t*> get_version;

    [NativeTypeName("cef_component_state_t (*)(struct _cef_component_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_t*, cef_component_state_t> get_state;
}
