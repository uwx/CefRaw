using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// CefClient implementation that returns meaningful handlers for
/// display, life-span, and load events. Equivalent to simple_handler_t
/// in the C implementation.
///
/// A global singleton is maintained so that GetDefaultClient() can
/// return the same instance to Chrome-style browser windows.
/// </summary>
public unsafe class SimpleClient : CefClient
{
    private static SimpleClient? s_instance;

    private readonly SimpleDisplayHandler _displayHandler;
    private readonly SimpleLifeSpanHandler _lifeSpanHandler;
    private readonly SimpleLoadHandler _loadHandler;

    /// <summary>
    /// True for Alloy style, false for Chrome style.
    /// </summary>
    public bool IsAlloyStyle { get; }

    /// <summary>
    /// List of open browser windows (shared with life-span handler).
    /// </summary>
    public List<ICefBrowser> Browsers => _lifeSpanHandler.Browsers;

    /// <summary>
    /// True when the main window is closing (set by DoClose).
    /// </summary>
    public bool IsClosing
    {
        get => _lifeSpanHandler.IsClosing;
        set => _lifeSpanHandler.IsClosing = value;
    }

    public SimpleClient(bool isAlloyStyle = false)
    {
        IsAlloyStyle = isAlloyStyle;

        _displayHandler = new SimpleDisplayHandler(this);
        _lifeSpanHandler = new SimpleLifeSpanHandler(this);
        _loadHandler = new SimpleLoadHandler(this);

        // Set global singleton.
        s_instance ??= this;
    }

    /// <summary>
    /// Returns the global singleton client instance (if created).
    /// Used by GetDefaultClient() for Chrome-style browser windows.
    /// </summary>
    public static SimpleClient? GetInstance() => s_instance;

    // ── Handler getters ─────────────────────────────────────────

    public override ICefDisplayHandler? GetDisplayHandler() => _displayHandler;
    public override ICefLifeSpanHandler? GetLifeSpanHandler() => _lifeSpanHandler;
    public override ICefLoadHandler? GetLoadHandler() => _loadHandler;

    // ── Remaining getters (return null) ─────────────────────────

    public override ICefAudioHandler? GetAudioHandler() => null;
    public override ICefCommandHandler? GetCommandHandler() => null;
    public override ICefContextMenuHandler? GetContextMenuHandler() => null;
    public override ICefDialogHandler? GetDialogHandler() => null;
    public override ICefDownloadHandler? GetDownloadHandler() => null;
    public override ICefDragHandler? GetDragHandler() => null;
    public override ICefFindHandler? GetFindHandler() => null;
    public override ICefFocusHandler? GetFocusHandler() => null;
    public override ICefFrameHandler? GetFrameHandler() => null;
    public override ICefPermissionHandler? GetPermissionHandler() => null;
    public override ICefJsdialogHandler? GetJsdialogHandler() => null;
    public override ICefKeyboardHandler? GetKeyboardHandler() => null;
    public override ICefPrintHandler? GetPrintHandler() => null;
    public override ICefRenderHandler? GetRenderHandler() => null;
    public override ICefRequestHandler? GetRequestHandler() => null;

    public override int OnProcessMessageReceived(
        ICefBrowser? browser, ICefFrame? frame,
        CefProcessId sourceProcess, ICefProcessMessage? message) => 0;

    // ── Cleanup ─────────────────────────────────────────────────

    protected override void OnRelease()
    {
        if (s_instance == this)
            s_instance = null;
        base.OnRelease();
    }
}
