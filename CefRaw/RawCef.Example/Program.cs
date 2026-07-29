using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Minimal CEF browser application — C# port of the "Complete Minimal Example"
/// from https://chromiumembedded.github.io/cef/using_the_capi.html.
/// </summary>
public static unsafe class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        // 1. Initialize the CEF library.
        Cef.InitializeLibrary();

        // 2. Create app (ref count starts at 1 internally).
        var app = new SimpleApp();

        // 3. Execute sub-processes. Both ExecuteSubProcess and Initialize
        //    will take ownership of a reference, so we need 2 total.
        app.AddRef(); // one extra ref for ExecuteSubProcess

        int exitCode = Cef.ExecuteSubProcess(app);
        if (exitCode >= 0)
            return exitCode; // Sub-process — app ref already released by ExecuteSubProcess.

        // 4. Initialize CEF for the browser process.
        var settings = new CefSettings();
        settings.NoSandbox = 1;
        settings.BrowserSubprocessPath = Cef.GetDefaultSubprocessPath();

        // Point CEF to its resource files (cef.pak, devtools_resources.pak, locales/).
        // These must be in the same directory as libcef.dll.
        var cefDir = AppContext.BaseDirectory;
        settings.ResourcesDirPath = cefDir;
        settings.LocalesDirPath = Path.Combine(cefDir, "locales");

        // Enable verbose logging to diagnose subprocess issues.
        settings.LogSeverity = CefLogSeverity.LOGSEVERITY_VERBOSE;
        settings.LogFile = Path.Combine(cefDir, "cef_debug.log");

        if (!Cef.Initialize(app, settings))
            return Cef.GetExitCode();

        // 5. Run the message loop (blocks until cef_quit_message_loop).
        Cef.RunMessageLoop();

        // 6. Shutdown.
        Cef.Shutdown();

        return 0;
    }
}
