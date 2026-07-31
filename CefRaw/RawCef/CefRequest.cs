namespace RawCef;

public partial class CefRequest
{
    public static ICefRequest Create()
    {
        return Cef.CreateRequest()!;
    }
}