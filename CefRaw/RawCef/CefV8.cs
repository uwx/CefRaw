using RawCef.Native;

namespace RawCef;

/// <summary>
/// Safe API surface for CEF V8 (JavaScript engine) bindings.
/// Wraps the low-level <see cref="CefUnsafe"/> V8 imports behind
/// discoverable methods returning managed <c>CefV8XxxRef</c> wrappers.
/// </summary>
public static unsafe class CefV8
{
    // ── Context ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns the current V8 context on the current thread, or <c>null</c>
    /// if no context is entered.
    /// </summary>
    public static CefV8ContextRef? GetCurrentContext()
    {
        var ptr = CefUnsafe.V8ContextGetCurrentContext();
        return ptr != null ? new CefV8ContextRef(ptr) : null;
    }

    /// <summary>
    /// Returns the entered V8 context on the current thread, or <c>null</c>
    /// if no context has been entered.
    /// </summary>
    public static CefV8ContextRef? GetEnteredContext()
    {
        var ptr = CefUnsafe.V8ContextGetEnteredContext();
        return ptr != null ? new CefV8ContextRef(ptr) : null;
    }

    /// <summary>
    /// Returns <c>true</c> if the current thread is executing within a V8 context.
    /// </summary>
    public static bool InContext()
    {
        return CefUnsafe.V8ContextInContext() != 0;
    }

    // ── Backing Store ────────────────────────────────────────────────

    /// <summary>
    /// Creates a new V8 backing store for use with <see cref="CreateArrayBufferFromBackingStore"/>.
    /// </summary>
    /// <param name="byteLength">The size of the backing store in bytes.</param>
    public static CefV8BackingStoreRef? CreateBackingStore(nuint byteLength)
    {
        var ptr = CefUnsafe.V8BackingStoreCreate(byteLength);
        return ptr != null ? new CefV8BackingStoreRef(ptr) : null;
    }

    // ── Value Factories ──────────────────────────────────────────────

    /// <summary>
    /// Creates a new V8 value of type undefined.
    /// </summary>
    public static CefV8ValueRef? CreateUndefined()
    {
        var ptr = CefUnsafe.V8ValueCreateUndefined();
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 value of type null.
    /// </summary>
    public static CefV8ValueRef? CreateNull()
    {
        var ptr = CefUnsafe.V8ValueCreateNull();
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 boolean value.
    /// </summary>
    public static CefV8ValueRef? CreateBool(bool value)
    {
        var ptr = CefUnsafe.V8ValueCreateBool(value ? 1 : 0);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 integer value.
    /// </summary>
    public static CefV8ValueRef? CreateInt(int value)
    {
        var ptr = CefUnsafe.V8ValueCreateInt(value);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 unsigned integer value.
    /// </summary>
    public static CefV8ValueRef? CreateUint(uint value)
    {
        var ptr = CefUnsafe.V8ValueCreateUint(value);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 double value.
    /// </summary>
    public static CefV8ValueRef? CreateDouble(double value)
    {
        var ptr = CefUnsafe.V8ValueCreateDouble(value);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 Date value from a <see cref="DateTime"/>.
    /// </summary>
    public static CefV8ValueRef? CreateDate(DateTime date)
    {
        var basetime = DateTimeToBasetime(date);
        var ptr = CefUnsafe.V8ValueCreateDate(basetime);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 string value. Returns a null V8 value if
    /// <paramref name="value"/> is <c>null</c>.
    /// </summary>
    public static CefV8ValueRef? CreateString(string? value)
    {
        if (value is null)
            return CreateNull();

        _cef_string_utf16_t str = default;
        fixed (char* p = value)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)value.Length, &str, copy: 1);
        }
        var ptr = CefUnsafe.V8ValueCreateString(&str);
        CefUnsafe.StringUtf16Clear(&str);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 object value with optional accessor and interceptor.
    /// </summary>
    /// <param name="accessor">Optional accessor for getting/setting properties.</param>
    /// <param name="interceptor">Optional interceptor for property access.</param>
    public static CefV8ValueRef? CreateObject(
        ICefV8Accessor? accessor = null,
        ICefV8Interceptor? interceptor = null)
    {
        var ptr = CefUnsafe.V8ValueCreateObject(
            accessor != null ? accessor.NativePtr : null,
            interceptor != null ? interceptor.NativePtr : null);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 array value with the specified <paramref name="length"/>.
    /// </summary>
    public static CefV8ValueRef? CreateArray(int length)
    {
        var ptr = CefUnsafe.V8ValueCreateArray(length);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 ArrayBuffer value that references the provided buffer.
    /// The optional <paramref name="releaseCallback"/> is invoked when the
    /// ArrayBuffer is garbage collected by V8.
    /// </summary>
    public static CefV8ValueRef? CreateArrayBuffer(
        ReadOnlySpan<byte> buffer,
        ICefV8ArrayBufferReleaseCallback? releaseCallback = null)
    {
        fixed (byte* p = buffer)
        {
            var ptr = CefUnsafe.V8ValueCreateArrayBuffer(
                p, (nuint)buffer.Length,
                releaseCallback != null ? releaseCallback.NativePtr : null);
            return ptr != null ? new CefV8ValueRef(ptr) : null;
        }
    }

    /// <summary>
    /// Creates a new V8 ArrayBuffer value with a copy of the provided buffer.
    /// The data is copied into a newly allocated buffer owned by V8.
    /// </summary>
    public static CefV8ValueRef? CreateArrayBufferWithCopy(ReadOnlySpan<byte> buffer)
    {
        fixed (byte* p = buffer)
        {
            var ptr = CefUnsafe.V8ValueCreateArrayBufferWithCopy(p, (nuint)buffer.Length);
            return ptr != null ? new CefV8ValueRef(ptr) : null;
        }
    }

    /// <summary>
    /// Creates a new V8 ArrayBuffer value from an existing backing store.
    /// </summary>
    public static CefV8ValueRef? CreateArrayBufferFromBackingStore(ICefV8BackingStore? backingStore)
    {
        var ptr = CefUnsafe.V8ValueCreateArrayBufferFromBackingStore(backingStore != null ? backingStore.NativePtr : null);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new V8 function value with the specified <paramref name="name"/>
    /// and <paramref name="handler"/>.
    /// </summary>
    public static CefV8ValueRef? CreateFunction(string name, ICefV8Handler handler)
    {
        _cef_string_utf16_t nameStr = default;
        fixed (char* p = name)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)name.Length, &nameStr, copy: 1);
        }
        var ptr = CefUnsafe.V8ValueCreateFunction(&nameStr, handler.NativePtr);
        CefUnsafe.StringUtf16Clear(&nameStr);
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new unresolved V8 Promise value.
    /// </summary>
    public static CefV8ValueRef? CreatePromise()
    {
        var ptr = CefUnsafe.V8ValueCreatePromise();
        return ptr != null ? new CefV8ValueRef(ptr) : null;
    }

    // ── Stack Trace ──────────────────────────────────────────────────

    /// <summary>
    /// Returns a snapshot of the current JavaScript stack trace.
    /// </summary>
    /// <param name="frameLimit">
    /// The maximum number of stack frames to capture. Use 0 for unlimited.
    /// </param>
    public static CefV8StackTraceRef? GetCurrentStackTrace(int frameLimit = 0)
    {
        var ptr = CefUnsafe.V8StackTraceGetCurrent(frameLimit);
        return ptr != null ? new CefV8StackTraceRef(ptr) : null;
    }

    // ── Extensions ───────────────────────────────────────────────────

    /// <summary>
    /// Register a new V8 extension with the specified JavaScript extension
    /// code and handler. Functions implemented by the handler are prototyped
    /// using the keyword 'native'. This function may only be called on the
    /// render process main thread.
    /// </summary>
    /// <returns>Returns <c>true</c> on success.</returns>
    /// <remarks>
    /// Example JavaScript extension code:
    /// <code>
    /// // create the 'example' global object if it doesn't already exist.
    /// if (!example)
    ///   example = {};
    /// (function() {
    ///   example.test.myfunction = function() {
    ///     native function MyFunction();
    ///     return MyFunction();
    ///   };
    /// })();
    /// </code>
    /// </remarks>
    public static bool RegisterExtension(
        string extensionName,
        string javascriptCode,
        ICefV8Handler handler)
    {
        _cef_string_utf16_t nameStr = default;
        _cef_string_utf16_t codeStr = default;

        fixed (char* pn = extensionName, pc = javascriptCode)
        {
            CefUnsafe.StringUtf16Set((ushort*)pn, (nuint)extensionName.Length, &nameStr, copy: 1);
            CefUnsafe.StringUtf16Set((ushort*)pc, (nuint)javascriptCode.Length, &codeStr, copy: 1);
        }

        var result = CefUnsafe.RegisterExtension(&nameStr, &codeStr, handler.NativePtr);

        CefUnsafe.StringUtf16Clear(&nameStr);
        CefUnsafe.StringUtf16Clear(&codeStr);

        return result != 0;
    }

    // ── Helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// The epoch for CEF basetime: January 1, 1601 UTC (Windows FILETIME epoch).
    /// </summary>
    private static readonly DateTime BasetimeEpoch = new(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="_cef_basetime_t"/>.
    /// </summary>
    private static _cef_basetime_t DateTimeToBasetime(DateTime dateTime)
    {
        var utc = dateTime.ToUniversalTime();
        var ticks = (utc - BasetimeEpoch).Ticks;
        // CEF basetime is microseconds; .NET ticks are 100-nanoseconds
        return new _cef_basetime_t { val = ticks / 10 };
    }
}
