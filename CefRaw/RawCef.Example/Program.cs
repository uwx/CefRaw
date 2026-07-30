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
    // ── Native-thread subprocess execution ─────────────────────────
    //
    // .NET worker threads have CET shadow stack entries from runtime init
    // that cause FAST_FAIL_CONTROL_INVALID_RETURN_ADDRESS in V8.
    // Using kernel32!CreateThread gives a thread with a clean shadow stack.

    private static _cef_app_t* s_subprocessApp;
    private static int s_subprocessExitCode;

    [DllImport("kernel32.dll")]
    private static extern void* GetModuleHandle(string? lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateThread(
        void* lpThreadAttributes, nuint dwStackSize,
        nint lpStartAddress,
        void* lpParameter, uint dwCreationFlags, out uint lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private const uint INFINITE = 0xFFFFFFFF;
    private const uint COINIT_MULTITHREADED = 0x0;

    [DllImport("ole32.dll")]
    private static extern int CoInitializeEx(void* pvReserved, uint dwCoInit);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static uint SubprocessThreadProc(void* param)
    {
        CoInitializeEx(null, COINIT_MULTITHREADED);
        s_subprocessApp->base.add_ref(&s_subprocessApp->base);

        _cef_main_args_t mainArgs = default;
        mainArgs.instance = GetModuleHandle(null);

        var result = CefUnsafe.ExecuteProcess(&mainArgs, s_subprocessApp, null);

        if (result >= 0)
            s_subprocessApp->base.release(&s_subprocessApp->base);

        s_subprocessExitCode = result;
        return 0;
    }
#endif

    [STAThread]  // Browser process UI thread needs STA for OLE/COM
    public static int Main(string[] args)
    {
        // 1. Initialize the CEF library.
        Cef.InitializeLibrary();

        // 2. Create app (ref count starts at 1 internally).
        var app = new SimpleApp();

        // 3. Execute sub-processes on a native OS thread (clean CET shadow stack).
#if OS_WIN
        s_subprocessApp = ((ICefApp)app).NativePtr;
        s_subprocessExitCode = -1;

        var fp = (delegate* unmanaged[Stdcall]<void*, uint>)&SubprocessThreadProc;
        var hThread = CreateThread(null, 0, (nint)fp, null, 0, out _);
        if (hThread == IntPtr.Zero)
            throw new InvalidOperationException(
                $"CreateThread failed: {Marshal.GetLastWin32Error()}");

        WaitForSingleObject(hThread, INFINITE);
        CloseHandle(hThread);

        if (s_subprocessExitCode >= 0)
            return s_subprocessExitCode;
#else
        app.AddRef();
        var exitCode = Cef.ExecuteSubProcess(app);
        if (exitCode >= 0)
            return exitCode;
#endif

        // 4. Initialize CEF for the browser process.
        var settings = new CefSettings();
        settings.NoSandbox = 1;
        settings.BrowserSubprocessPath = Cef.GetDefaultSubprocessPath();

        var cefDir = AppContext.BaseDirectory;
        settings.ResourcesDirPath = cefDir;
        settings.LocalesDirPath = Path.Combine(cefDir, "locales");
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
