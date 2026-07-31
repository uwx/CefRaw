namespace RawCef;

public partial class CefCommandLine
{
    /// <summary>
    /// Creates a new command line object.
    /// </summary>
    public static ICefCommandLine Create()
    {
        return Cef.CreateCommandLine()!;
    }
}
