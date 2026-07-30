using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// Load handler that displays an error page on load failure for
/// Alloy-style browsers.
/// Equivalent to load_handler_t callbacks in the C code.
/// </summary>
public unsafe class SimpleLoadHandler : CefLoadHandler
{
    private readonly SimpleClient _parent;

    public SimpleLoadHandler(SimpleClient parent)
    {
        _parent = parent;
    }

    /// <summary>
    /// Called when a page fails to load. For Alloy style, display a
    /// custom error page via a data: URI.
    /// </summary>
    public override void OnLoadError(ICefBrowser? browser, ICefFrame? frame,
        CefErrorcode errorCode, string? errorText, string? failedUrl)
    {
        // Only show error page in Alloy style, and don't show for ABORTED.
        if (!_parent.IsAlloyStyle || errorCode == CefErrorcode.ERR_ABORTED)
            return;

        if (frame is null)
            return;

        // Build an error page HTML.
        var errorHtml = $"<html><body bgcolor=\"white\">" +
                        $"<h2>Failed to load URL {failedUrl ?? "(unknown)"} " +
                        $"with error {(int)errorCode} ({errorCode}).</h2>" +
                        $"<p>{errorText ?? ""}</p></body></html>";

        // Base64 encode and create a data: URI.
        var bytes = System.Text.Encoding.UTF8.GetBytes(errorHtml);
        var base64 = Convert.ToBase64String(bytes);
        var dataUri = $"data:text/html;base64,{base64}";

        frame.LoadUrl(dataUri);
    }

    // ── Remaining load handler overrides (no-op) ───────────────

    public override void OnLoadingStateChange(ICefBrowser? arg0, int arg1, int arg2, int arg3) { }
    public override void OnLoadStart(ICefBrowser? arg0, ICefFrame? arg1, CefTransitionType arg2) { }
    public override void OnLoadEnd(ICefBrowser? arg0, ICefFrame? arg1, int arg2) { }
}
