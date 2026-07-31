namespace RawCef;

public partial class CefImage
{
    /// <summary>
    /// Creates a new empty CEF image.
    /// </summary>
    public static ICefImage Create()
    {
        return Cef.CreateImage()!;
    }
}
