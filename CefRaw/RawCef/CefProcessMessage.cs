namespace RawCef;

public partial class CefProcessMessage
{
    /// <summary>
    /// Creates a new CEF process message with the specified <paramref name="name"/>.
    /// </summary>
    public static ICefProcessMessage Create(string name)
    {
        return Cef.CreateProcessMessage(name)!;
    }
}
