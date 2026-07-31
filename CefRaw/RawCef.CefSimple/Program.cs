using System.Runtime.InteropServices;
using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// C# port of the cefsimple_capi example — a minimal but feature-rich
/// CEF browser application demonstrating display, life-span, and load
/// handlers with platform-specific window title support.
///
/// Works on Windows and Linux via #if OS_WIN / #if OS_LINUX blocks.
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

    [STAThread]
    public static int Main(string[] args)
    {
        // 1. Configure CEF API version.
        Cef.InitializeLibrary();

        // 2. Create app (ref count starts at 1 internally).
        var app = new SimpleApp();

        // 3. Execute sub-process on an MTA thread.
        //    The browser process UI thread must be STA (for OLE/COM); GPU and
        //    renderer subprocesses need MTA (for DirectX). We use a worker thread.
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
        var settings = new CefSettings
        {
            NoSandbox = 1,
            BrowserSubprocessPath = Cef.GetDefaultSubprocessPath()
        };

        var cefDir = AppContext.BaseDirectory;
        settings.ResourcesDirPath = cefDir;
        settings.LocalesDirPath = Path.Combine(cefDir, "locales");
        settings.LogSeverity = CefLogSeverity.Verbose;
        settings.LogFile = Path.Combine(cefDir, "cef_debug.log");

        if (!Cef.Initialize(app, settings))
            return Cef.GetExitCode();

        // 5. Run the CEF message loop (blocks until quit).
        Cef.RunMessageLoop();

        // 6. Shutdown.
        Cef.Shutdown();

        return 0;
    }
}
