namespace RawCef;

public partial class CefPostDataElement
{
    /// <summary>
    /// Creates a new CEF post data element object.
    /// </summary>
    public static ICefPostDataElement Create()
    {
        return Cef.CreatePostDataElement()!;
    }
}
