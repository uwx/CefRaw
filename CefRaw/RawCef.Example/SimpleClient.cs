using RawCef;
using RawCef.Native;

namespace RawCef.Example;

/// <summary>
/// Minimal CefClient implementation that returns null for all handler getters.
/// Uses default CEF behavior for all browser events.
/// </summary>
public unsafe class SimpleClient : CefClient
{
    public override ICefAudioHandler? GetAudioHandler() => null;
    public override ICefCommandHandler? GetCommandHandler() => null;
    public override ICefContextMenuHandler? GetContextMenuHandler() => null;
    public override ICefDialogHandler? GetDialogHandler() => null;
    public override ICefDisplayHandler? GetDisplayHandler() => null;
    public override ICefDownloadHandler? GetDownloadHandler() => null;
    public override ICefDragHandler? GetDragHandler() => null;
    public override ICefFindHandler? GetFindHandler() => null;
    public override ICefFocusHandler? GetFocusHandler() => null;
    public override ICefFrameHandler? GetFrameHandler() => null;
    public override ICefPermissionHandler? GetPermissionHandler() => null;
    public override ICefJsdialogHandler? GetJsdialogHandler() => null;
    public override ICefKeyboardHandler? GetKeyboardHandler() => null;
    public override ICefLifeSpanHandler? GetLifeSpanHandler() => null;
    public override ICefLoadHandler? GetLoadHandler() => null;
    public override ICefPrintHandler? GetPrintHandler() => null;
    public override ICefRenderHandler? GetRenderHandler() => null;
    public override ICefRequestHandler? GetRequestHandler() => null;

    public override int OnProcessMessageReceived(
        ICefBrowser? browser, ICefFrame? frame,
        CefProcessId sourceProcess, ICefProcessMessage? message) => 0;
}
