namespace RawCef;

public partial class CefListValue
{
    /// <summary>
    /// Creates a new empty CEF list value.
    /// </summary>
    public static ICefListValue Create()
    {
        return Cef.CreateListValue()!;
    }
}
