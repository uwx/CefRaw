namespace RawCef.Native;

[NativeTypeName("struct CefBaseTime : cef_basetime_t")]
public unsafe partial struct CefBaseTime
{
    public _cef_basetime_t Base;

    public CefBaseTime()
    {
        cef_basetime_t = new _cef_basetime_t
        {
        };
    }

    public CefBaseTime([NativeTypeName("const cef_basetime_t &")] _cef_basetime_t* value)
    {
        cef_basetime_t = value;
    }

    public static CefBaseTime Now()
    {
        return cef_basetime_now();
    }
}
