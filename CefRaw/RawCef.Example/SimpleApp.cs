using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Minimal CefApp implementation that returns no custom handlers.
/// All abstract methods return null, using CEF default behavior.
/// </summary>
public unsafe class SimpleApp : CefApp
{
    public override ICefBrowserProcessHandler? GetBrowserProcessHandler() => null;

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
