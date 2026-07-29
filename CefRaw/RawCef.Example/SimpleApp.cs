using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Minimal CefApp implementation that creates a browser window on startup.
/// </summary>
public unsafe class SimpleApp : CefApp
{
    private readonly SimpleBrowserProcessHandler _browserProcessHandler;

    public SimpleApp()
    {
        var client = new SimpleClient();
        _browserProcessHandler = new SimpleBrowserProcessHandler(client);
    }

    public override ICefBrowserProcessHandler? GetBrowserProcessHandler() => _browserProcessHandler;

    public override ICefRenderProcessHandler? GetRenderProcessHandler() => null;

    public override ICefResourceBundleHandler? GetResourceBundleHandler() => null;

    public override void OnBeforeCommandLineProcessing(string? processType, ICefCommandLine? commandLine)
    {
        // No custom command-line processing needed.
    }

    public override void OnRegisterCustomSchemes(ICefSchemeRegistrar? registrar)
    {
        // No custom schemes to register.
    }
}
