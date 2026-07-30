using System.Runtime.InteropServices;
using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Minimal CEF browser application — C# port of the "Complete Minimal Example"
/// from https://chromiumembedded.github.io/cef/using_the_capi.html.
/// </summary>
public static unsafe class Program
{
#if OS_WIN
    private const uint COINIT_APARTMENTTHREADED = 0x2;
    private const uint COINIT_MULTITHREADED = 0x0;

    [DllImport("ole32.dll")]
    private static extern void CoUninitialize();

    [DllImport("ole32.dll")]
    private static extern int CoInitializeEx(void* pvReserved, uint dwCoInit);
#endif

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
        //    must not pre-initialize COM as STA.
        //
        //    IMPORTANT: .NET's COM initialization on worker threads can leave the
        //    CET (Control-flow Enforcement Technology) shadow stack in a state
        //    that conflicts with V8, causing FAST_FAIL_CONTROL_INVALID_RETURN_ADDRESS.
        //    We fix this by tearing down .NET's COM init and reinitializing COM
        //    cleanly before calling cef_execute_process.
        app.AddRef(); // extra ref for ExecuteSubProcess (released internally)

#if OS_WIN
        // Undo .NET's COM initialization which may have corrupted the
        // CET shadow stack state, then reinitialize COM cleanly.
        CoUninitialize();
        CoInitializeEx(null, COINIT_MULTITHREADED);
#endif
        var exitCode = Cef.ExecuteSubProcess(app);
        if (exitCode >= 0)
            return exitCode; // Sub-process completed.

#if OS_WIN
        // Undo .NET's COM initialization which may have corrupted the
        // CET shadow stack state, then reinitialize COM cleanly.
        CoUninitialize();
        CoInitializeEx(null, COINIT_APARTMENTTHREADED);
#endif

        // 4. Initialize CEF for the browser process.
        var settings = new CefSettings();
        settings.NoSandbox = 1;
        settings.BrowserSubprocessPath = Cef.GetDefaultSubprocessPath();

        // Point CEF to its resource files (chrome_100_percent.pak, resources.pak, locales/).
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
