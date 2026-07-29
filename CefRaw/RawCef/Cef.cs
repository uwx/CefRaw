using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using RawCef.Native;

namespace RawCef;

/// <summary>
/// Clean API surface for the Chromium Embedded Framework.
/// Wraps the low-level <see cref="CefUnsafe"/> imports behind
/// discoverable, safe-feeling methods.
/// </summary>
public static unsafe class Cef
{
    // ── Library init ─────────────────────────────────────────────────

    /// <summary>
    /// Configures the CEF API version. Must be called before any other
    /// CEF function. On macOS, call this after <c>cef_load_library()</c>.
    /// </summary>
    public static void InitializeLibrary()
    {
        CefUnsafe.ApiHash(CefUnsafe.CEF_API_VERSION, 0);
    }

    // ── Process management ───────────────────────────────────────────

    /// <summary>
    /// Executes CEF sub-process logic. Returns the sub-process exit code
    /// (≥ 0) if this process is a CEF sub-process; returns -1 if this is
    /// the main browser process and execution should continue.
    /// </summary>
    /// <remarks>
    /// <paramref name="app"/> must have at least 2 references before this
    /// call — one for <see cref="ExecuteSubProcess"/> and one for
    /// <see cref="Initialize"/>.
    /// </remarks>
    public static int ExecuteSubProcess(CefApp? app = null)
    {
        _cef_main_args_t mainArgs = BuildMainArgs();
        _cef_app_t* appPtr = app is not null ? ((ICefApp)app).NativePtr : null;
        var result = CefUnsafe.ExecuteProcess(&mainArgs, appPtr, null);
        return result;
    }

    /// <summary>
    /// Initializes CEF for the main browser process.
    /// </summary>
    /// <returns>
    /// <c>true</c> on success; <c>false</c> if initialization failed.
    /// On failure, call <see cref="GetExitCode"/> for details.
    /// </returns>
    /// <remarks>
    /// <paramref name="app"/> transfers one reference to CEF on success.
    /// Do not release it after a successful call.
    /// </remarks>
    public static bool Initialize(CefApp app, ICefSettings? settings = null)
    {
        _cef_main_args_t mainArgs = BuildMainArgs();
        return CefUnsafe.Initialize(&mainArgs, settings != null ? settings.NativePtr : null, ((ICefApp)app).NativePtr, null) != 0;
    }

    // ── Message loop ─────────────────────────────────────────────────

    /// <summary>
    /// Runs the CEF message loop on the current thread. Blocks until
    /// <see cref="Shutdown"/> or <c>cef_quit_message_loop()</c> is called.
    /// </summary>
    public static void RunMessageLoop()
    {
        CefUnsafe.RunMessageLoop();
    }

    /// <summary>
    /// Requests CEF to shut down and exit the message loop.
    /// </summary>
    public static void Shutdown()
    {
        CefUnsafe.Shutdown();
    }

    /// <summary>
    /// Returns the CEF exit code, which can be used after
    /// <see cref="Initialize"/> fails or after shutdown.
    /// </summary>
    public static int GetExitCode()
    {
        return CefUnsafe.GetExitCode();
    }

    // ── Browser creation ─────────────────────────────────────────────

    /// <summary>
    /// Creates a new browser window asynchronously. The browser will be
    /// created on the UI thread and delivered via the life-span handler's
    /// <c>OnAfterCreated</c> callback.
    /// </summary>
    public static bool CreateBrowser(
        CefClient client,
        string startUrl,
        ICefWindowInfo? windowInfo = null,
        _cef_browser_settings_t* browserSettings = null)
    {
        _cef_string_utf16_t urlStr = default;
        fixed (char* p = startUrl)
        {
            CefUnsafe.StringUtf16Set((ushort*)p, (nuint)startUrl.Length, &urlStr, copy: 1);
        }

        var result = CefUnsafe.BrowserHostCreateBrowser(
            windowInfo != null ? windowInfo.NativePtr : null,
            ((ICefClient)client).NativePtr,
            &urlStr,
            browserSettings,
            null,   // extra_info
            null    // request_context
        );

        CefUnsafe.StringUtf16Clear(&urlStr);
        return result != 0;
    }

    // ── Subprocess path detection ────────────────────────────────────

    /// <summary>
    /// Returns the path CEF should use to launch subprocesses (GPU, renderer,
    /// network, etc.). On Windows, returns the current process path. If running
    /// via <c>dotnet run</c>, you must set <c>BrowserSubprocessPath</c> manually
    /// or publish as self-contained.
    /// </summary>
    public static string GetDefaultSubprocessPath()
    {
#if OS_WIN
        var path = Environment.ProcessPath;
        // If we're running via dotnet.exe, the subprocess must be a self-contained exe.
        // Warn if the process looks like the .NET host rather than a standalone app.
        if (path is not null && Path.GetFileNameWithoutExtension(path).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
        {
            // Running via dotnet run — CEF subprocesses won't work. Publish as self-contained.
            Debug.Fail("CEF subprocesses require a self-contained publish when using dotnet CLI. " +
                       "Publish with: dotnet publish -r win-x64 --self-contained");
        }
        return path ?? "";
#else
        return Environment.ProcessPath ?? "";
#endif
    }

    private static _cef_main_args_t BuildMainArgs()
    {
        _cef_main_args_t args = default;
#if OS_WIN
        args.instance = GetModuleHandle(null);
#else
        // LEAKS UNMANAGED MEMORY !!!!!
        var managedArgs = Environment.GetCommandLineArgs();
        args.argc = managedArgs.Length;
        args.argv = (sbyte**)Marshal.AllocHGlobal(sizeof(sbyte*) * managedArgs.Length);
        for (var i = 0; i < managedArgs.Length; i++)
        {
            var utf8 = Encoding.UTF8.GetBytes(managedArgs[i]);
            args.argv[i] = (sbyte*)Marshal.AllocHGlobal(utf8.Length);
            utf8.CopyTo(new Span<byte>(args.argv[i], utf8.Length));
        }
#endif
        return args;
    }

#if OS_WIN
    [DllImport("kernel32.dll")]
    private static extern void* GetModuleHandle(string? lpModuleName);
#endif
}
