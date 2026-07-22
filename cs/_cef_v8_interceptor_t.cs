namespace RawCef.Native;

public unsafe partial struct _cef_v8_interceptor_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("int (*)(struct _cef_v8_interceptor_t *, const cef_string_t *, struct _cef_v8_value_t *, struct _cef_v8_value_t **, cef_string_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_interceptor_t*, _cef_string_utf16_t*, _cef_v8_value_t*, _cef_v8_value_t**, _cef_string_utf16_t*, int> get_byname;

    [NativeTypeName("int (*)(struct _cef_v8_interceptor_t *, int, struct _cef_v8_value_t *, struct _cef_v8_value_t **, cef_string_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_interceptor_t*, int, _cef_v8_value_t*, _cef_v8_value_t**, _cef_string_utf16_t*, int> get_byindex;

    [NativeTypeName("int (*)(struct _cef_v8_interceptor_t *, const cef_string_t *, struct _cef_v8_value_t *, struct _cef_v8_value_t *, cef_string_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_interceptor_t*, _cef_string_utf16_t*, _cef_v8_value_t*, _cef_v8_value_t*, _cef_string_utf16_t*, int> set_byname;

    [NativeTypeName("int (*)(struct _cef_v8_interceptor_t *, int, struct _cef_v8_value_t *, struct _cef_v8_value_t *, cef_string_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_interceptor_t*, int, _cef_v8_value_t*, _cef_v8_value_t*, _cef_string_utf16_t*, int> set_byindex;
}
