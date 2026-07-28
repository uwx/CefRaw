using System.Runtime.CompilerServices;

namespace RawCef.Native;

[NativeTypeName("struct CefTime : cef_time_t")]
public unsafe partial struct CefTime
{
    public _cef_time_t Base;

    public CefTime()
    {
        cef_time_t = new _cef_time_t
        {
        };
    }

    public CefTime([NativeTypeName("const cef_time_t &")] _cef_time_t* r)
    {
        cef_time_t = r;
    }

    public CefTime([NativeTypeName("time_t")] long r)
    {
        SetTimeT(r);
    }

    public CefTime(double r)
    {
        SetDoubleT(r);
    }

    public void SetTimeT([NativeTypeName("time_t")] long r)
    {
        _ = cef_time_from_timet(r, (_cef_time_t*)Unsafe.AsPointer(ref this));
    }

    [return: NativeTypeName("time_t")]
    public readonly long GetTimeT()
    {
        long time = 0;

        _ = cef_time_to_timet((_cef_time_t*)Unsafe.AsPointer(in this), &time);
        return time;
    }

    public void SetDoubleT(double r)
    {
        _ = cef_time_from_doublet(r, (_cef_time_t*)Unsafe.AsPointer(ref this));
    }

    public readonly double GetDoubleT()
    {
        double time = 0;

        _ = cef_time_to_doublet((_cef_time_t*)Unsafe.AsPointer(in this), &time);
        return time;
    }

    public void Now()
    {
        _ = cef_time_now((_cef_time_t*)Unsafe.AsPointer(ref this));
    }

    [return: NativeTypeName("long long")]
    public long Delta([NativeTypeName("const CefTime &")] CefTime* other)
    {
        long delta = 0;

        _ = cef_time_delta((_cef_time_t*)Unsafe.AsPointer(in this), other, &delta);
        return delta;
    }
}
