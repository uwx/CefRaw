using System.Runtime.InteropServices;
using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// Display handler that updates the native window title when the page
/// title changes (Alloy style only).
/// Equivalent to display_handler_t callbacks in the C code.
/// </summary>
public unsafe class SimpleDisplayHandler : CefDisplayHandler
{
    private readonly SimpleClient _parent;

    public SimpleDisplayHandler(SimpleClient parent)
    {
        _parent = parent;
    }

    public override void OnTitleChange(ICefBrowser? browser, string? title)
    {
        if (!_parent.IsAlloyStyle)
            return;

        if (browser is null || title is null)
            return;

        PlatformSetWindowTitle(browser, title);
    }

    /// <summary>
    /// Platform-specific window title update.
    /// On Windows: calls SetWindowTextW with the UTF-16 title.
    /// On Linux (X11): sets _NET_WM_NAME property on the X11 window.
    /// </summary>
    private static void PlatformSetWindowTitle(ICefBrowser browser, string title)
    {
#if OS_WIN
        var host = browser.GetHost();
        if (host is null) return;

        var hwnd = host.GetWindowHandle();
        if (hwnd != null)
        {
            SetWindowTextW(hwnd, title);
        }
#elif OS_LINUX
        // Linux X11 title change would use cef_get_xdisplay() and XChangeProperty.
        // This requires P/Invoke into libX11.so — omitted for now.
        // A full implementation would look like the simple_handler_linux.c code.
#endif
    }

#if OS_WIN
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern unsafe int SetWindowTextW(void* hWnd, string lpString);
#endif

    // ── Remaining display handler overrides (no-op) ────────────

    public override void OnAddressChange(ICefBrowser? arg0, ICefFrame? arg1, string? arg2) { }
    public override void OnFaviconUrlchange(ICefBrowser? arg0, ICefStringList? arg1) { }
    public override void OnFullscreenModeChange(ICefBrowser? arg0, int arg1) { }
    public override int OnTooltip(ICefBrowser? arg0, out string? arg1) { arg1 = null; return 0; }
    public override void OnStatusMessage(ICefBrowser? arg0, string? arg1) { }
    public override int OnConsoleMessage(ICefBrowser? arg0, CefLogSeverity arg1, string? arg2, string? arg3, int arg4) => 0;
    public override int OnAutoResize(ICefBrowser? arg0, ICefSize? arg1) => 0;
    public override void OnLoadingProgressChange(ICefBrowser? arg0, double arg1) { }
    public override int OnCursorChange(ICefBrowser? arg0, void* arg1, CefCursorType arg2, ICefCursorInfo? arg3) => 0;
    public override void OnMediaAccessChange(ICefBrowser? arg0, int arg1, int arg2) { }
    public override int OnContentsBoundsChange(ICefBrowser? arg0, ICefRect? arg1) => 0;
    public override int GetRootWindowScreenRect(ICefBrowser? arg0, ICefRect? arg1) => 0;
}
