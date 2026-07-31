using RawCef.Native;

namespace RawCef;

/// <summary>
/// Utility safe bindings for CEF — time functions, bool helpers,
/// scheme registration, and string conversion.
/// </summary>
public static unsafe partial class Cef
{
    // ── Cert / Thread / Task Helpers ─────────────────────────────────

    /// <summary>
    /// Returns <c>true</c> if the certificate status represents an error.
    /// </summary>
    public static bool IsCertStatusError(CefCertStatus status)
    {
        return CefUnsafe.IsCertStatusError(status) != 0;
    }

    /// <summary>
    /// Returns <c>true</c> if the calling thread is the specified CEF thread.
    /// </summary>
    public static bool CurrentlyOn(CefThreadId threadId)
    {
        return CefUnsafe.CurrentlyOn(threadId) != 0;
    }

    /// <summary>
    /// Posts a task for execution on the specified CEF thread.
    /// Returns <c>true</c> on success.
    /// </summary>
    public static bool PostTask(CefThreadId threadId, ICefTask task)
    {
        return CefUnsafe.PostTask(threadId, task.NativePtr) != 0;
    }

    /// <summary>
    /// Posts a task for delayed execution on the specified CEF thread.
    /// Returns <c>true</c> on success.
    /// </summary>
    /// <param name="delayMs">The delay in milliseconds.</param>
    public static bool PostDelayedTask(CefThreadId threadId, ICefTask task, long delayMs)
    {
        return CefUnsafe.PostDelayedTask(threadId, task.NativePtr, delayMs) != 0;
    }

    // ── Scheme Handler ───────────────────────────────────────────────

    /// <summary>
    /// Clears all registered scheme handler factories.
    /// Returns <c>true</c> on success.
    /// </summary>
    public static bool ClearSchemeHandlerFactories()
    {
        return CefUnsafe.ClearSchemeHandlerFactories() != 0;
    }

    /// <summary>
    /// Registers a scheme handler factory for the specified
    /// <paramref name="schemeName"/> and optional <paramref name="domainName"/>.
    /// Returns <c>true</c> on success.
    /// </summary>
    /// <param name="domainName">
    /// An optional domain name to restrict the handler to.
    /// Pass <c>null</c> or empty to handle all domains for the scheme.
    /// </param>
    public static bool RegisterSchemeHandlerFactory(
        string schemeName,
        string? domainName,
        ICefSchemeHandlerFactory factory)
    {
        _cef_string_utf16_t schemeStr = default;
        _cef_string_utf16_t domainStr = default;

        fixed (char* ps = schemeName)
        {
            CefUnsafe.StringUtf16Set((ushort*)ps, (nuint)schemeName.Length, &schemeStr, copy: 1);
        }

        _cef_string_utf16_t* domainPtr = null;
        if (domainName is not null)
        {
            fixed (char* pd = domainName)
            {
                CefUnsafe.StringUtf16Set((ushort*)pd, (nuint)domainName.Length, &domainStr, copy: 1);
            }
            domainPtr = &domainStr;
        }

        var result = CefUnsafe.RegisterSchemeHandlerFactory(
            &schemeStr, domainPtr, factory.NativePtr);

        CefUnsafe.StringUtf16Clear(&schemeStr);
        if (domainPtr != null)
            CefUnsafe.StringUtf16Clear(&domainStr);

        return result != 0;
    }

    // ── Time Functions ───────────────────────────────────────────────

    /// <summary>
    /// Returns the current system time as a <see cref="DateTime"/> in UTC.
    /// </summary>
    public static DateTime TimeNow()
    {
        _cef_time_t time = default;
        CefUnsafe.TimeNow(&time);
        return FromCefTime(&time);
    }

    /// <summary>
    /// Returns the current system time as a CEF basetime
    /// (microseconds since January 1, 1601 UTC).
    /// </summary>
    public static DateTime BasetimeNow()
    {
        var basetime = CefUnsafe.BasetimeNow();
        _cef_time_t time = default;
        CefUnsafe.TimeFromBasetime(basetime, &time);
        return FromCefTime(&time);
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a time_t (Unix timestamp in seconds).
    /// </summary>
    public static long TimeToTimet(DateTime time)
    {
        _cef_time_t cefTime = default;
        FillCefTime(time, &cefTime);
        long result = 0;
        CefUnsafe.TimeToTimet(&cefTime, &result);
        return result;
    }

    /// <summary>
    /// Converts a time_t (Unix timestamp in seconds) to a <see cref="DateTime"/> in UTC.
    /// </summary>
    public static DateTime TimeFromTimet(long timeT)
    {
        _cef_time_t cefTime = default;
        CefUnsafe.TimeFromTimet(timeT, &cefTime);
        return FromCefTime(&cefTime);
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a double (seconds since epoch).
    /// </summary>
    public static double TimeToDoublet(DateTime time)
    {
        _cef_time_t cefTime = default;
        FillCefTime(time, &cefTime);
        double result = 0;
        CefUnsafe.TimeToDoublet(&cefTime, &result);
        return result;
    }

    /// <summary>
    /// Converts a double (seconds since epoch) to a <see cref="DateTime"/> in UTC.
    /// </summary>
    public static DateTime TimeFromDoublet(double doubleT)
    {
        _cef_time_t cefTime = default;
        CefUnsafe.TimeFromDoublet(doubleT, &cefTime);
        return FromCefTime(&cefTime);
    }

    /// <summary>
    /// Computes the time difference between two <see cref="DateTime"/> values.
    /// </summary>
    public static TimeSpan TimeDelta(DateTime time1, DateTime time2)
    {
        _cef_time_t t1 = default, t2 = default;
        FillCefTime(time1, &t1);
        FillCefTime(time2, &t2);
        long delta = 0;
        CefUnsafe.TimeDelta(&t1, &t2, &delta);
        return TimeSpan.FromMilliseconds(delta);
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="_cef_basetime_t"/>.
    /// </summary>
    public static _cef_basetime_t TimeToBasetime(DateTime time)
    {
        _cef_time_t cefTime = default;
        FillCefTime(time, &cefTime);
        _cef_basetime_t result = default;
        CefUnsafe.TimeToBasetime(&cefTime, &result);
        return result;
    }

    /// <summary>
    /// Converts a <see cref="_cef_basetime_t"/> to a <see cref="DateTime"/> in UTC.
    /// </summary>
    public static DateTime TimeFromBasetime(_cef_basetime_t basetime)
    {
        _cef_time_t cefTime = default;
        CefUnsafe.TimeFromBasetime(basetime, &cefTime);
        return FromCefTime(&cefTime);
    }

    // ── String Conversion ────────────────────────────────────────────

    /// <summary>
    /// Converts a UTF-16 string to lowercase using ICU rules.
    /// Returns <c>null</c> if the conversion fails.
    /// </summary>
    public static string? StringUtf16ToLower(string input)
    {
        _cef_string_utf16_t output = default;
        fixed (char* p = input)
        {
            if (CefUnsafe.StringUtf16ToLower((ushort*)p, (nuint)input.Length, &output) == 0)
                return null;
        }
        return CefStringRef.ToStringAndFree(&output);
    }

    /// <summary>
    /// Converts a UTF-16 string to uppercase using ICU rules.
    /// Returns <c>null</c> if the conversion fails.
    /// </summary>
    public static string? StringUtf16ToUpper(string input)
    {
        _cef_string_utf16_t output = default;
        fixed (char* p = input)
        {
            if (CefUnsafe.StringUtf16ToUpper((ushort*)p, (nuint)input.Length, &output) == 0)
                return null;
        }
        return CefStringRef.ToStringAndFree(&output);
    }

    // ── Time Helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Fills a <see cref="_cef_time_t"/> struct from a <see cref="DateTime"/>.
    /// The time is converted to UTC before filling.
    /// </summary>
    private static void FillCefTime(DateTime dateTime, _cef_time_t* time)
    {
        var utc = dateTime.ToUniversalTime();
        time->Year = utc.Year;
        time->Month = utc.Month;
        time->DayOfWeek = (int)utc.DayOfWeek; // Sunday = 0, matches CEF tm_wday
        time->DayOfMonth = utc.Day;
        time->Hour = utc.Hour;
        time->Minute = utc.Minute;
        time->Second = utc.Second;
        time->Millisecond = utc.Millisecond;
    }

    /// <summary>
    /// Creates a <see cref="DateTime"/> in UTC from a <see cref="_cef_time_t"/> struct.
    /// </summary>
    private static DateTime FromCefTime(_cef_time_t* time)
    {
        return new DateTime(
            time->Year, time->Month, time->DayOfMonth,
            time->Hour, time->Minute, time->Second, time->Millisecond,
            DateTimeKind.Utc);
    }
}
