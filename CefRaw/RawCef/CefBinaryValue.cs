using RawCef.Native;

namespace RawCef;

public partial class CefBinaryValue
{
    /// <summary>
    /// Creates a new object that is not owned by any other object. The specified
    /// |data| will be copied.
    /// </summary>
    public static ICefBinaryValue Create(ReadOnlySpan<byte> data)
    {
        return Cef.CreateBinaryValue(data)!;
    }
}