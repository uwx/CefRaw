namespace RawCef.Native;

public partial struct _cef_browser_view_t
{
}

public partial struct _cef_browser_view_t
{
}

public partial struct _cef_browser_view_t
{
}

public unsafe partial struct _cef_browser_view_t
{
    [NativeTypeName("cef_view_t")]
    public _cef_view_t @base;

    [NativeTypeName("struct _cef_browser_t *(*)(struct _cef_browser_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_browser_view_t*, _cef_browser_t*> get_browser;

    [NativeTypeName("struct _cef_view_t *(*)(struct _cef_browser_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_browser_view_t*, _cef_view_t*> get_chrome_toolbar;

    [NativeTypeName("void (*)(struct _cef_browser_view_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_browser_view_t*, int, void> set_prefer_accelerators;

    [NativeTypeName("cef_runtime_style_t (*)(struct _cef_browser_view_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_browser_view_t*, cef_runtime_style_t> get_runtime_style;
}
