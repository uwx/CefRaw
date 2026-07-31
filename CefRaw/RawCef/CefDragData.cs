namespace RawCef;

public partial class CefDragData
{
    public static ICefDragData Create()
    {
        return Cef.CreateDragData()!;
    }
}