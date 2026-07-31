using RawCef.Native;

namespace RawCef;

public unsafe partial class CefBrowser
{
    /// <summary>
    /// Creates a new browser synchronously. This method should only be called
    /// on the CEF UI thread.
    /// </summary>
    public static ICefBrowser CreateSync(
        CefClient client,
        string startUrl,
        ICefWindowInfo? windowInfo = null,
        ICefBrowserSettings? browserSettings = null,
        ICefDictionaryValue? extraInfo = null,
        ICefRequestContext? requestContext = null)
    {
        return Cef.CreateBrowserSync(client, startUrl, windowInfo, browserSettings, extraInfo, requestContext)!;
    }
}
