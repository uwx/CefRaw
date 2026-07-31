using RawCef.Native;

namespace RawCef;

/// <summary>
/// Factory and accessor safe bindings for CEF types.
/// These wrap the low-level <c>Create*</c> and <c>Get*</c> functions
/// from <see cref="CefUnsafe"/> that return library-owned native pointers
/// as managed <c>CefXxxRef</c> wrappers.
/// </summary>
public static unsafe partial class Cef
{
    // ── Value / Binary / Dictionary / List ───────────────────────────

    /// <summary>
    /// Creates a new CEF value object.
    /// </summary>
    public static CefValueRef? CreateValue()
    {
        var ptr = CefUnsafe.ValueCreate();
        return ptr != null ? new CefValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new CEF binary value from the provided data.
    /// </summary>
    public static CefBinaryValueRef? CreateBinaryValue(ReadOnlySpan<byte> data)
    {
        fixed (byte* p = data)
        {
            var ptr = CefUnsafe.BinaryValueCreate(p, (nuint)data.Length);
            return ptr != null ? new CefBinaryValueRef(ptr) : null;
        }
    }

    /// <summary>
    /// Creates a new empty CEF dictionary value.
    /// </summary>
    public static CefDictionaryValueRef? CreateDictionaryValue()
    {
        var ptr = CefUnsafe.DictionaryValueCreate();
        return ptr != null ? new CefDictionaryValueRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new empty CEF list value.
    /// </summary>
    public static CefListValueRef? CreateListValue()
    {
        var ptr = CefUnsafe.ListValueCreate();
        return ptr != null ? new CefListValueRef(ptr) : null;
    }

    // ── Image ────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new empty CEF image.
    /// </summary>
    public static CefImageRef? CreateImage()
    {
        var ptr = CefUnsafe.ImageCreate();
        return ptr != null ? new CefImageRef(ptr) : null;
    }

    // ── Stream Reader ────────────────────────────────────────────────

    /// <summary>
    /// Creates a stream reader that reads from the specified file.
    /// </summary>
    public static CefStreamReaderRef? CreateStreamReaderForFile(string fileName)
    {
        _cef_string_utf16_t str = default;
        fixed (char* p = fileName)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)fileName.Length, &str, copy: 1);
        }
        var ptr = CefUnsafe.StreamReaderCreateForFile(&str);
        CefUnsafe.StringUtf16Clear(&str);
        return ptr != null ? new CefStreamReaderRef(ptr) : null;
    }

    /// <summary>
    /// Creates a stream reader that reads from the provided data.
    /// </summary>
    public static CefStreamReaderRef? CreateStreamReaderForData(ReadOnlySpan<byte> data)
    {
        fixed (byte* p = data)
        {
            var ptr = CefUnsafe.StreamReaderCreateForData(p, (nuint)data.Length);
            return ptr != null ? new CefStreamReaderRef(ptr) : null;
        }
    }

    /// <summary>
    /// Creates a stream reader that reads from the provided handler.
    /// </summary>
    public static CefStreamReaderRef? CreateStreamReaderForHandler(ICefReadHandler handler)
    {
        var ptr = CefUnsafe.StreamReaderCreateForHandler(handler.NativePtr);
        return ptr != null ? new CefStreamReaderRef(ptr) : null;
    }

    // ── Stream Writer ────────────────────────────────────────────────

    /// <summary>
    /// Creates a stream writer that writes to the specified file.
    /// </summary>
    public static CefStreamWriterRef? CreateStreamWriterForFile(string fileName)
    {
        _cef_string_utf16_t str = default;
        fixed (char* p = fileName)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)fileName.Length, &str, copy: 1);
        }
        var ptr = CefUnsafe.StreamWriterCreateForFile(&str);
        CefUnsafe.StringUtf16Clear(&str);
        return ptr != null ? new CefStreamWriterRef(ptr) : null;
    }

    /// <summary>
    /// Creates a stream writer that writes to the provided handler.
    /// </summary>
    public static CefStreamWriterRef? CreateStreamWriterForHandler(ICefWriteHandler handler)
    {
        var ptr = CefUnsafe.StreamWriterCreateForHandler(handler.NativePtr);
        return ptr != null ? new CefStreamWriterRef(ptr) : null;
    }

    // ── Drag / Process / Request / Post ──────────────────────────────

    /// <summary>
    /// Creates a new CEF drag data object.
    /// </summary>
    public static CefDragDataRef? CreateDragData()
    {
        var ptr = CefUnsafe.DragDataCreate();
        return ptr != null ? new CefDragDataRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new CEF process message with the specified <paramref name="name"/>.
    /// </summary>
    public static CefProcessMessageRef? CreateProcessMessage(string name)
    {
        _cef_string_utf16_t str = default;
        fixed (char* p = name)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)name.Length, &str, copy: 1);
        }
        var ptr = CefUnsafe.ProcessMessageCreate(&str);
        CefUnsafe.StringUtf16Clear(&str);
        return ptr != null ? new CefProcessMessageRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new CEF request object.
    /// </summary>
    public static CefRequestRef? CreateRequest()
    {
        var ptr = CefUnsafe.RequestCreate();
        return ptr != null ? new CefRequestRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new CEF post data object.
    /// </summary>
    public static CefPostDataRef? CreatePostData()
    {
        var ptr = CefUnsafe.PostDataCreate();
        return ptr != null ? new CefPostDataRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new CEF post data element object.
    /// </summary>
    public static CefPostDataElementRef? CreatePostDataElement()
    {
        var ptr = CefUnsafe.PostDataElementCreate();
        return ptr != null ? new CefPostDataElementRef(ptr) : null;
    }

    // ── Cookie / Media ───────────────────────────────────────────────

    /// <summary>
    /// Returns the global cookie manager. If <paramref name="callback"/> is
    /// non-null, it will be executed on the IO thread after the manager's
    /// storage is initialized.
    /// </summary>
    public static CefCookieManagerRef? GetGlobalCookieManager(
        ICefCompletionCallback? callback = null)
    {
        var ptr = CefUnsafe.CookieManagerGetGlobalManager(callback != null ? callback.NativePtr : null);
        return ptr != null ? new CefCookieManagerRef(ptr) : null;
    }

    /// <summary>
    /// Returns the global media router. If <paramref name="callback"/> is
    /// non-null, it will be executed on the UI thread after the router is
    /// initialized.
    /// </summary>
    public static CefMediaRouterRef? GetGlobalMediaRouter(
        ICefCompletionCallback? callback = null)
    {
        var ptr = CefUnsafe.MediaRouterGetGlobal(callback != null ? callback.NativePtr : null);
        return ptr != null ? new CefMediaRouterRef(ptr) : null;
    }

    // ── Preference Manager ───────────────────────────────────────────

    /// <summary>
    /// Returns the global preference manager.
    /// </summary>
    public static CefPreferenceManagerRef? GetGlobalPreferenceManager()
    {
        var ptr = CefUnsafe.PreferenceManagerGetGlobal();
        return ptr != null ? new CefPreferenceManagerRef(ptr) : null;
    }

    /// <summary>
    /// Retrieves the current Chrome variations as command-line switches.
    /// </summary>
    public static string[] GetChromeVariationsAsSwitches()
    {
        using var list = new CefStringListRef();
        CefUnsafe.PreferenceManagerGetChromeVariationsAsSwitches(
            ((ICefStringList)list).NativePtr);
        return StringListToArray(list);
    }

    /// <summary>
    /// Retrieves the current Chrome variations as strings.
    /// </summary>
    public static string[] GetChromeVariationsAsStrings()
    {
        using var list = new CefStringListRef();
        CefUnsafe.PreferenceManagerGetChromeVariationsAsStrings(
            ((ICefStringList)list).NativePtr);
        return StringListToArray(list);
    }

    // ── Request Context ──────────────────────────────────────────────

    /// <summary>
    /// Returns the global request context.
    /// </summary>
    public static CefRequestContextRef? GetGlobalRequestContext()
    {
        var ptr = CefUnsafe.RequestContextGetGlobalContext();
        return ptr != null ? new CefRequestContextRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new request context with the specified settings and handler.
    /// </summary>
    public static CefRequestContextRef? CreateRequestContext(
        ICefRequestContextSettings? settings = null,
        ICefRequestContextHandler? handler = null)
    {
        var ptr = CefUnsafe.RequestContextCreateContext(
            settings != null ? settings.NativePtr : null,
            handler != null ? handler.NativePtr : null);
        return ptr != null ? new CefRequestContextRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new request context that shares storage with
    /// <paramref name="other"/> and uses the specified <paramref name="handler"/>.
    /// </summary>
    public static CefRequestContextRef? CreateRequestContextShared(
        ICefRequestContext other,
        ICefRequestContextHandler? handler = null)
    {
        var ptr = CefUnsafe.RequestContextCreateContextShared(
            other.NativePtr,
            handler != null ? handler.NativePtr : null);
        return ptr != null ? new CefRequestContextRef(ptr) : null;
    }

    // ── Browser ──────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new browser synchronously. This method should only be called
    /// on the CEF UI thread.
    /// </summary>
    public static CefBrowserRef? CreateBrowserSync(
        CefClient client,
        string startUrl,
        ICefWindowInfo? windowInfo = null,
        ICefBrowserSettings? browserSettings = null,
        ICefDictionaryValue? extraInfo = null,
        ICefRequestContext? requestContext = null)
    {
        _cef_string_utf16_t urlStr = default;
        fixed (char* p = startUrl)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)startUrl.Length, &urlStr, copy: 1);
        }

        var ptr = CefUnsafe.BrowserHostCreateBrowserSync(
            windowInfo != null ? windowInfo.NativePtr : null,
            ((ICefClient)client).NativePtr,
            &urlStr,
            browserSettings != null ? browserSettings.NativePtr : null,
            extraInfo != null ? extraInfo.NativePtr : null,
            requestContext != null ? requestContext.NativePtr : null);

        CefUnsafe.StringUtf16Clear(&urlStr);
        return ptr != null ? new CefBrowserRef(ptr) : null;
    }

    /// <summary>
    /// Returns the browser with the specified <paramref name="browserId"/>,
    /// or <c>null</c> if no matching browser is found.
    /// </summary>
    public static CefBrowserRef? GetBrowserByIdentifier(int browserId)
    {
        var ptr = CefUnsafe.BrowserHostGetBrowserByIdentifier(browserId);
        return ptr != null ? new CefBrowserRef(ptr) : null;
    }

    // ── Menu / Print / Response ──────────────────────────────────────

    /// <summary>
    /// Creates a new menu model with the specified <paramref name="delegate"/>.
    /// </summary>
    public static CefMenuModelRef? CreateMenuModel(ICefMenuModelDelegate? @delegate = null)
    {
        var ptr = CefUnsafe.MenuModelCreate(@delegate != null ? @delegate.NativePtr : null);
        return ptr != null ? new CefMenuModelRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new print settings object.
    /// </summary>
    public static CefPrintSettingsRef? CreatePrintSettings()
    {
        var ptr = CefUnsafe.PrintSettingsCreate();
        return ptr != null ? new CefPrintSettingsRef(ptr) : null;
    }

    /// <summary>
    /// Creates a new response object.
    /// </summary>
    public static CefResponseRef? CreateResponse()
    {
        var ptr = CefUnsafe.ResponseCreate();
        return ptr != null ? new CefResponseRef(ptr) : null;
    }

    // ── Command Line ─────────────────────────────────────────────────

    /// <summary>
    /// Creates a new command line object.
    /// </summary>
    public static CefCommandLineRef? CreateCommandLine()
    {
        var ptr = CefUnsafe.CommandLineCreate();
        return ptr != null ? new CefCommandLineRef(ptr) : null;
    }

    /// <summary>
    /// Returns the global command line object.
    /// </summary>
    public static CefCommandLineRef? GetGlobalCommandLine()
    {
        var ptr = CefUnsafe.CommandLineGetGlobal();
        return ptr != null ? new CefCommandLineRef(ptr) : null;
    }

    // ── Task Runner ──────────────────────────────────────────────────

    /// <summary>
    /// Returns the task runner for the current thread.
    /// </summary>
    public static CefTaskRunnerRef? GetTaskRunnerForCurrentThread()
    {
        var ptr = CefUnsafe.TaskRunnerGetForCurrentThread();
        return ptr != null ? new CefTaskRunnerRef(ptr) : null;
    }

    /// <summary>
    /// Returns the task runner for the specified <paramref name="threadId"/>.
    /// </summary>
    public static CefTaskRunnerRef? GetTaskRunnerForThread(CefThreadId threadId)
    {
        var ptr = CefUnsafe.TaskRunnerGetForThread(threadId);
        return ptr != null ? new CefTaskRunnerRef(ptr) : null;
    }

    // ── Helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="CefStringListRef"/> to a managed string array.
    /// </summary>
    private static string[] StringListToArray(CefStringListRef list)
    {
        var result = new string[list.Count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = list.GetValue(i) ?? string.Empty;
        }
        return result;
    }
}
