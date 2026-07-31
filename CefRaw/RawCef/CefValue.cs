namespace RawCef;

public partial class CefValue
{
    /// <summary>
    /// Creates a new CEF value object.
    /// </summary>
    public static ICefValue Create()
    {
        return Cef.CreateValue()!;
    }
}
