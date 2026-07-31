namespace RawCef;

public partial class CefDictionaryValue
{
    /// <summary>
    /// Creates a new empty CEF dictionary value.
    /// </summary>
    public static ICefDictionaryValue Create()
    {
        return Cef.CreateDictionaryValue()!;
    }
}
