using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Handles browser-process lifecycle events. Creates the initial browser
/// window when the CEF context is ready.
/// </summary>
public unsafe class SimpleBrowserProcessHandler : CefBrowserProcessHandler
{
    private readonly string _startUrl;
    private readonly SimpleClient _client;

    public SimpleBrowserProcessHandler(SimpleClient client, string startUrl = "https://www.example.com")
    {
        _client = client;
        _startUrl = startUrl;
    }

    /// <summary>
    /// Called on the UI thread after CEF has initialized. This is the right
    /// time to create the first browser window.
    /// </summary>
    public override void OnContextInitialized()
    {
        Cef.CreateBrowser(_client, _startUrl);
    }

    // ── Remaining abstract overrides (default no-op / null) ──────────

    public override void OnRegisterCustomPreferences(CefPreferencesType arg0, ICefPreferenceRegistrar? arg1) { }

    public override void OnBeforeChildProcessLaunch(ICefCommandLine? arg0) { }

    public override int OnAlreadyRunningAppRelaunch(ICefCommandLine? arg0, string? arg1) => 0;

    public override void OnScheduleMessagePumpWork(long arg0) { }

    public override ICefClient? GetDefaultClient() => null;

    public override ICefRequestContextHandler? GetDefaultRequestContextHandler() => null;
}
