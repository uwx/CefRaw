namespace RawCef.Native;

public unsafe partial struct _cef_v8_backing_store_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void *(*)(struct _cef_v8_backing_store_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_backing_store_t*, void*> data;

    [NativeTypeName("size_t (*)(struct _cef_v8_backing_store_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_backing_store_t*, nuint> byte_length;

    [NativeTypeName("int (*)(struct _cef_v8_backing_store_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_backing_store_t*, int> is_valid;
}
