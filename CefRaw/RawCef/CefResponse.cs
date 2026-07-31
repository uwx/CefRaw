namespace RawCef;

public partial class CefResponse
{
    /// <summary>
    /// Creates a new response object.
    /// </summary>
    public static ICefResponse Create()
    {
        return Cef.CreateResponse()!;
    }
}
