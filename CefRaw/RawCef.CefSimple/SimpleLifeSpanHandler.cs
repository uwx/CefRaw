using RawCef;
using RawCef.Native;

namespace RawCef.CefSimple;

/// <summary>
/// Life-span handler that tracks open browsers and quits the
/// message loop when the last browser closes.
/// Equivalent to life_span_handler_t callbacks in the C code.
/// </summary>
public unsafe class SimpleLifeSpanHandler : CefLifeSpanHandler
{
    private readonly SimpleClient _parent;
    private readonly List<ICefBrowser> _browsers = new();

    public SimpleClient Parent => _parent;
    public List<ICefBrowser> Browsers => _browsers;
    public bool IsClosing { get; set; }

    public SimpleLifeSpanHandler(SimpleClient parent)
    {
        _parent = parent;
    }

    /// <summary>
    /// Called after a browser is created. Add it to the list.
    /// </summary>
    public override void OnAfterCreated(ICefBrowser? browser)
    {
        if (browser is null) return;

        lock (_browsers)
        {
            _browsers.Add(browser);
            // Add a reference for the list.
            browser.AddRef();
        }
    }

    /// <summary>
    /// Called when a browser is about to close. For the last browser,
    /// set the closing flag to allow the close.
    /// </summary>
    public override int DoClose(ICefBrowser? browser)
    {
        lock (_browsers)
        {
            if (_browsers.Count == 1)
                IsClosing = true;
        }

        // Return 0 (false) to allow the close.
        return 0;
    }

    /// <summary>
    /// Called right before a browser is destroyed. Remove it from the list.
    /// If no browsers remain, quit the message loop.
    /// </summary>
    public override void OnBeforeClose(ICefBrowser? browser)
    {
        if (browser is null) return;

        lock (_browsers)
        {
            // Use GetIdentifier() for stable identity — pointer and
            // reference equality are not guaranteed across callbacks.
            var id = browser.GetIdentifier();
            var index = _browsers.FindIndex(b => b.GetIdentifier() == id);
            if (index >= 0)
            {
                _browsers.RemoveAt(index);
                // Release the list's reference.
                browser.Release();
            }
        }

        lock (_browsers)
        {
            if (_browsers.Count == 0)
            {
                // All browsers closed — quit the application.
                Cef.QuitMessageLoop();
            }
        }
    }

    // ── Remaining life-span handler overrides ──────────────────

    public override int OnBeforePopup(ICefBrowser? arg0, ICefFrame? arg1, int arg2, string? arg3, string? arg4,
        CefWindowOpenDisposition arg5, int arg6, ref CefPopupFeatures arg7, ICefWindowInfo? arg8,
        out ICefClient? arg9, ICefBrowserSettings? arg10, out ICefDictionaryValue? arg11, int* arg12)
    {
        arg9 = null;
        arg11 = null;
        return 0; // Return false — allow the popup, creating a new browser
    }

    public override void OnBeforePopupAborted(ICefBrowser? arg0, int arg1) { }

    public override void OnBeforeDevToolsPopup(ICefBrowser? arg0, ICefWindowInfo? arg1, out ICefClient? arg2,
        ICefBrowserSettings? arg3, out ICefDictionaryValue? arg4, int* arg5)
    {
        arg2 = null;
        arg4 = null;
    }
}
