namespace RawCef;

public partial class CefRequestContext
{
    /// <summary>
    /// Creates a new request context with the specified settings and handler.
    /// </summary>
    public static ICefRequestContext Create(
        ICefRequestContextSettings? settings = null,
        ICefRequestContextHandler? handler = null)
    {
        return Cef.CreateRequestContext(settings, handler)!;
    }

    /// <summary>
    /// Creates a new request context that shares storage with
    /// <paramref name="other"/> and uses the specified <paramref name="handler"/>.
    /// </summary>
    public static ICefRequestContext CreateShared(
        ICefRequestContext other,
        ICefRequestContextHandler? handler = null)
    {
        return Cef.CreateRequestContextShared(other, handler)!;
    }
}
