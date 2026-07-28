namespace RawCef.Native;

public unsafe partial struct _cef_v8_array_buffer_release_callback_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("void (*)(struct _cef_v8_array_buffer_release_callback_t *, void *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_v8_array_buffer_release_callback_t*, void*, void> release_buffer;
}
