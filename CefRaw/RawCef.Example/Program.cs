using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Minimal CEF browser application — C# port of the "Complete Minimal Example"
/// from https://chromiumembedded.github.io/cef/using_the_capi.html.
/// </summary>
public static unsafe class Program
{
    [STAThread]  // Browser process UI thread needs STA for OLE/COM (drag-drop, clipboard, etc.)
    public static int Main(string[] args)
    {
        // 1. Initialize the CEF library (process-global, no COM dependency).
        Cef.InitializeLibrary();

        // 2. Create app (ref count starts at 1 internally).
        var app = new SimpleApp();

        // 3. Execute sub-processes on an MTA thread. GPU, renderer, and utility
        //    subprocesses need MTA for DirectX and other COM APIs. The browser
        //    process UI thread needs STA (set by [STAThread]), but subprocesses
        //    must not pre-initialize COM as STA — so we dispatch them to a
        //    dedicated MTA thread.
        app.AddRef(); // extra ref for ExecuteSubProcess (released internally)

        int exitCode = -1;
        Exception? subprocessException = null;
        using var subprocessDone = new ManualResetEventSlim();

        var subprocessThread = new Thread(() =>
        {
            try
            {
                exitCode = Cef.ExecuteSubProcess(app);
            }
            catch (Exception ex)
            {
                subprocessException = ex;
            }
            finally
            {
                subprocessDone.Set();
            }
        });
        #pragma warning disable CA1416 // SetApartmentState is Windows-only (CEF COM is Windows-only)
        subprocessThread.SetApartmentState(ApartmentState.MTA);
        #pragma warning restore CA1416
        subprocessThread.Start();
        subprocessDone.Wait();

        if (subprocessException is not null)
            throw new AggregateException("Subprocess execution failed.", subprocessException);

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
