namespace RawCef;

public partial class CefPrintSettings
{
    /// <summary>
    /// Creates a new print settings object.
    /// </summary>
    public static ICefPrintSettings Create()
    {
        return Cef.CreatePrintSettings()!;
    }
}
