namespace RawCef;

public partial class CefMenuModel
{
    /// <summary>
    /// Creates a new menu model with the specified <paramref name="delegate"/>.
    /// </summary>
    public static ICefMenuModel Create(ICefMenuModelDelegate? @delegate = null)
    {
        return Cef.CreateMenuModel(@delegate)!;
    }
}
