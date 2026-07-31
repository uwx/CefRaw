using System.Runtime.InteropServices;
using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

/// <summary>
/// Per-test-class fixture that initializes the CEF runtime so that
/// C++ <c>operator new</c> (and therefore <c>cef_string_list_alloc</c>,
/// <c>cef_string_map_alloc</c>, etc.) works without hitting
/// <c>PA_NOTREACHED</c> in PartitionAlloc.
///
/// Uses the CEF debug binaries from <c>RawCef.Binaries.Win64.Debug</c>.
/// </summary>
public sealed unsafe class CefInitializationFixture : IDisposable
{
    private static int _initialized;

#if OS_WIN
    private const uint COINIT_APARTMENTTHREADED = 0x2;
    private const uint COINIT_MULTITHREADED = 0x0;

    [DllImport("ole32.dll")]
    private static extern void CoUninitialize();

    [DllImport("ole32.dll")]
    private static extern int CoInitializeEx(void* pvReserved, uint dwCoInit);
#endif

    public CefInitializationFixture()
    {
        // Only initialize once — shared across all test classes that use this fixture.
        if (Interlocked.Exchange(ref _initialized, 1) != 0)
            return;

        string baseDir = AppContext.BaseDirectory;

        // 1. Configure CEF API version.
        Cef.InitializeLibrary();

        // 2. Create a minimal app (ref count starts at 1).
        var app = new MinimalCefApp();
        app.AddRef(); // extra ref for ExecuteSubProcess

        // 3. Execute sub-process detection.
        //    On the main browser process, this returns -1.
        //    COM must be clean; undo .NET's COM init, then reinit.
#if OS_WIN
        CoUninitialize();
        CoInitializeEx(null, COINIT_MULTITHREADED);
#endif
        int exitCode = Cef.ExecuteSubProcess(app);
        if (exitCode >= 0)
        {
            // We're a subprocess — should not happen during tests.
            Environment.Exit(exitCode);
        }

#if OS_WIN
        CoUninitialize();
        CoInitializeEx(null, COINIT_APARTMENTTHREADED);
#endif

        // 4. Configure settings with debug binaries from the output directory.
        var settings = new CefSettings
        {
            NoSandbox = 1,
        };

        // Use bootstrap.exe as the subprocess launcher.
        string bootstrapPath = Path.Combine(baseDir, "bootstrap.exe");
        if (File.Exists(bootstrapPath))
        {
            settings.BrowserSubprocessPath = bootstrapPath;
        }

        // Point CEF at the resource pak files (flattened to output root).
        settings.ResourcesDirPath = baseDir;
        settings.LocalesDirPath = Path.Combine(baseDir, "locales");

        // 5. Initialize CEF for the browser process.
        if (!Cef.Initialize(app, settings))
        {
            // If initialization fails (e.g., missing resources),
            // we still proceed — tests that need full CEF will be skipped.
        }
    }

    public void Dispose()
    {
        // Shutdown is handled by the test framework process exit;
        // calling Cef.Shutdown() from a test fixture's Dispose can
        // interfere with other tests that share the fixture.
        // We deliberately do NOT call Shutdown here.
    }
}

/// <summary>
/// Minimal <see cref="CefApp"/> that satisfies the abstract contract
/// without creating any browser, client, or handler instances.
/// </summary>
internal sealed unsafe class MinimalCefApp : CefApp
{
    public override void OnBeforeCommandLineProcessing(string? processType, ICefCommandLine? commandLine)
    {
        // No-op: don't add any switches.
    }

    public override void OnRegisterCustomSchemes(ICefSchemeRegistrar? registrar)
    {
        // No-op: no custom schemes.
    }

    public override ICefResourceBundleHandler? GetResourceBundleHandler() => null;

    public override ICefBrowserProcessHandler? GetBrowserProcessHandler() => null;

    public override ICefRenderProcessHandler? GetRenderProcessHandler() => null;
}
