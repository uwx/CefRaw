using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// Handles browser-process lifecycle. Creates the initial browser
/// window when CEF context is initialized.
/// Equivalent to browser_process_handler_t callbacks in the C code.
/// </summary>
public unsafe class SimpleBrowserProcessHandler : CefBrowserProcessHandler
{
    private readonly SimpleClient _client;
    private readonly string _startUrl;

    public SimpleBrowserProcessHandler(SimpleClient client, string? startUrl = null)
    {
        _client = client;

        // Check for --url=... command-line switch.
        var startUrlFromArgs = startUrl;
        if (string.IsNullOrEmpty(startUrlFromArgs))
        {
            var clArgs = Environment.GetCommandLineArgs();
            foreach (var arg in clArgs)
            {
                if (arg.StartsWith("--url=", StringComparison.OrdinalIgnoreCase))
                {
                    startUrlFromArgs = arg["--url=".Length..];
                    break;
                }
            }
        }

        _startUrl = !string.IsNullOrEmpty(startUrlFromArgs) ? startUrlFromArgs : "https://www.google.com";
    }

    public override void OnContextInitialized()
    {
        // Create the browser settings.
        var browserSettings = new CefBrowserSettings();

        // Create window info.
        var windowInfo = new CefWindowInfo();

#if OS_WIN
        // On Windows, specify window styles for a normal top-level window.
        const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
        const uint WS_CLIPCHILDREN    = 0x02000000;
        const uint WS_CLIPSIBLINGS    = 0x04000000;
        const uint WS_VISIBLE         = 0x10000000;
        const int  CW_USEDEFAULT      = unchecked((int)0x80000000);

        var wi = (ICefWindowInfo)windowInfo;
        wi.Style = WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_VISIBLE;
        windowInfo.Bounds = new _cef_rect_t { x = CW_USEDEFAULT, y = CW_USEDEFAULT, width = CW_USEDEFAULT, height = CW_USEDEFAULT };
        windowInfo.WindowName = "cefsimple_capi";
#elif OS_LINUX
        windowInfo.Bounds = new _cef_rect_t { x = 0, y = 0, width = 800, height = 600 };
#endif

        // Use default runtime style (Chrome by default).
        windowInfo.RuntimeStyle = CefRuntimeStyle.Default;

        // Create the browser. CEF takes ownership of the native pointer
        // (calls add_ref internally). Do NOT call AddRef() here — our
        // constructor already gave the SimpleClient ref count 1, and CEF
        // will add its own reference.
        Cef.CreateBrowser(_client, _startUrl, windowInfo, browserSettings);
    }

    // ── Remaining overrides ─────────────────────────────────────

    public override void OnRegisterCustomPreferences(CefPreferencesType arg0, ICefPreferenceRegistrar? arg1) { }

    public override void OnBeforeChildProcessLaunch(ICefCommandLine? arg0) { }

    public override int OnAlreadyRunningAppRelaunch(ICefCommandLine? arg0, string? arg1) => 0;

    public override void OnScheduleMessagePumpWork(long arg0) { }

    /// <summary>
    /// Returns the default client for Chrome-style UI.
    /// Uses the global singleton SimpleClient instance.
    /// </summary>
    public override ICefClient? GetDefaultClient()
    {
        // Return the global singleton (matches C++ SimpleApp::GetDefaultClient).
        var instance = SimpleClient.GetInstance();
        if (instance is not null)
            instance.AddRef(); // CEF will release when done
        return instance;
    }

    public override ICefRequestContextHandler? GetDefaultRequestContextHandler() => null;
}
