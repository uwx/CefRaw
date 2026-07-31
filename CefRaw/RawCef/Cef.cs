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
public static unsafe partial class Cef
{
    private static int _isShutdown;

    /// <summary>
    /// Returns <c>true</c> after <see cref="Shutdown"/> has been called.
    /// Native CEF calls should be avoided once this returns <c>true</c>.
    /// </summary>
    public static bool IsShutdown => Volatile.Read(ref _isShutdown) != 0;

    // ── Library init ─────────────────────────────────────────────────

    /// <summary>
    /// Configures the CEF API version. Must be called before any other
    /// CEF function. On macOS, call this after <c>cef_load_library()</c>.
    /// </summary>
    public static void InitializeLibrary()
    {
        CefUnsafe.ApiHash(CefUnsafe.CEF_API_VERSION, 0);
        Volatile.Write(ref _isShutdown, 0);
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
    public static int ExecuteSubProcess(CefApp app)
    {
        _cef_main_args_t mainArgs = BuildMainArgs();
        var result = CefUnsafe.ExecuteProcess(&mainArgs, ((ICefApp)app).NativePtr, null);
        if (result >= 0)
            app.Release(); // cef_execute_process consumed its reference
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
    /// Run the CEF message loop. Use this function instead of an application-
    /// provided message loop to get the best balance between performance and CPU
    /// usage. This function should only be called on the main application thread
    /// and only if cef_initialize() is called with a
    /// cef_settings_t.multi_threaded_message_loop value of false (0). This function
    /// will block until a quit message is received by the system.
    /// </summary>
    public static void RunMessageLoop()
    {
        CefUnsafe.RunMessageLoop();
    }

    /// <summary>
    /// Requests CEF to shut down and exit the message loop.
    /// After this call, native CEF pointers are no longer valid
    /// and <see cref="IsShutdown"/> returns <c>true</c>.
    /// </summary>
    public static void Shutdown()
    {
        Volatile.Write(ref _isShutdown, 1);
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
    /// Create a new browser using the window parameters specified by |windowInfo|.
    /// All values will be copied internally and the actual window (if any) will be
    /// created on the UI thread. If |request_context| is NULL the global request
    /// context will be used. This function can be called on any browser process
    /// thread and will not block. The optional |extra_info| parameter provides an
    /// opportunity to specify extra information specific to the created browser
    /// that will be passed to cef_render_process_handler_t::on_browser_created() in
    /// the render process.
    /// </summary>
    public static bool CreateBrowser(
        CefClient client,
        string startUrl,
        ICefWindowInfo? windowInfo = null,
        ICefBrowserSettings? browserSettings = null)
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
            browserSettings != null ? browserSettings.NativePtr : null,
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
        args.Instance = GetModuleHandle(null);
#else
        // LEAKS UNMANAGED MEMORY !!!!!
        var managedArgs = Environment.GetCommandLineArgs();
        args.Argc = managedArgs.Length;
        args.Argv = (sbyte**)Marshal.AllocHGlobal(sizeof(sbyte*) * managedArgs.Length);
        for (var i = 0; i < managedArgs.Length; i++)
        {
            var utf8 = Encoding.UTF8.GetBytes(managedArgs[i]);
            args.Argv[i] = (sbyte*)Marshal.AllocHGlobal(utf8.Length);
            utf8.CopyTo(new Span<byte>(args.Argv[i], utf8.Length));
        }
#endif
        return args;
    }

#if OS_WIN
    [DllImport("kernel32.dll")]
    private static extern void* GetModuleHandle(string? lpModuleName);
#endif

    /// <summary>
    /// Perform a single iteration of CEF message loop processing. This function is
    /// provided for cases where the CEF message loop must be integrated into an
    /// existing application message loop. Use of this function is not recommended
    /// for most users; use either the cef_run_message_loop() function or
    /// cef_settings_t.multi_threaded_message_loop if possible. When using this
    /// function care must be taken to balance performance against excessive CPU
    /// usage. It is recommended to enable the cef_settings_t.external_message_pump
    /// option when using this function so that
    /// cef_browser_process_handler_t::on_schedule_message_pump_work() callbacks can
    /// facilitate the scheduling process. This function should only be called on
    /// the main application thread and only if cef_initialize() is called with a
    /// cef_settings_t.multi_threaded_message_loop value of false (0). This function
    /// will not block.
    /// </summary>
    public static void DoMessageLoopWork()
    {
        CefUnsafe.DoMessageLoopWork();
    }

    /// <summary>
    /// Quit the CEF message loop that was started by calling
    /// cef_run_message_loop(). This function should only be called on the main
    /// application thread and only if cef_run_message_loop() was used.
    /// </summary>
    public static void QuitMessageLoop()
    {
        CefUnsafe.QuitMessageLoop();
    }

    /// <summary>
    /// Set to true (1) before calling OS APIs on the CEF UI thread that will enter
    /// a native message loop (see usage restrictions below). Set to false (0) after
    /// exiting the native message loop. On Windows, use the CefSetOSModalLoop
    /// function instead in cases like native top menus where resize of the browser
    /// content is not required, or in cases like printer APIs where reentrancy
    /// safety cannot be guaranteed.
    /// Nested processing of Chromium tasks is disabled by default because common
    /// controls and/or printer functions may use nested native message loops that
    /// lead to unplanned reentrancy. This function re-enables nested processing in
    /// the scope of an upcoming native message loop. It must only be used in cases
    /// where the stack is reentrancy safe and processing nestable tasks is
    /// explicitly safe. Do not use in cases (like the printer example) where an OS
    /// API may experience unplanned reentrancy as a result of a new task executing
    /// immediately.
    /// For instance,
    /// - The UI thread is running a message loop.
    /// - It receives a task #1 and executes it.
    /// - The task #1 implicitly starts a nested message loop. For example, via
    ///   Windows APIs such as MessageBox or GetSaveFileName, or default handling of
    ///   a user-initiated drag/resize operation (e.g. DefWindowProc handling of
    ///   WM_SYSCOMMAND for SC_MOVE/SC_SIZE).
    /// - The UI thread receives a task #2 before or while in this second message
    ///   loop.
    /// - With NestableTasksAllowed set to true (1), the task #2 will run right
    ///   away. Otherwise, it will be executed right after task #1 completes at
    ///   "thread message loop level".
    /// </summary>
    /// <param name="allowed"></param>
    public static void SetNestableTasksAllowed(bool allowed)
    {
        CefUnsafe.SetNestableTasksAllowed(allowed ? 1 : 0);
    }
}
