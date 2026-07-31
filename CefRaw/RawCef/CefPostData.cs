namespace RawCef;

public partial class CefPostData
{
    public static ICefPostData Create()
    {
        return Cef.CreatePostData()!;
    }
}