namespace RawCef.Native;

public unsafe partial struct _cef_component_updater_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("size_t (*)(struct _cef_component_updater_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_updater_t*, nuint> get_component_count;

    [NativeTypeName("void (*)(struct _cef_component_updater_t *, size_t *, struct _cef_component_t **) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_updater_t*, nuint*, _cef_component_t**, void> get_components;

    [NativeTypeName("struct _cef_component_t *(*)(struct _cef_component_updater_t *, const cef_string_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_updater_t*, _cef_string_utf16_t*, _cef_component_t*> get_component_by_id;

    [NativeTypeName("void (*)(struct _cef_component_updater_t *, const cef_string_t *, cef_component_update_priority_t, struct _cef_component_update_callback_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_component_updater_t*, _cef_string_utf16_t*, cef_component_update_priority_t, _cef_component_update_callback_t*, void> update;
}
