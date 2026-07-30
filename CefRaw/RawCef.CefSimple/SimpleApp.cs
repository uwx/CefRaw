using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// Minimal CefApp that creates a browser on context initialization.
/// Equivalent to simple_app_t in the C implementation.
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
        // Disable GPU to avoid common subprocess crashes.
        // IMPORTANT: Only add --disable-gpu for the browser process.
        // Adding it to the renderer process can destabilize V8's JIT
        // and cause __fastfail crashes during JavaScript deoptimization.
        if (string.IsNullOrEmpty(processType))
        {
            commandLine?.AppendSwitch("--disable-gpu");
        }
    }

    public override void OnRegisterCustomSchemes(ICefSchemeRegistrar? registrar)
    {
        // No custom schemes.
    }
}
