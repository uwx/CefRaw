namespace RawCef;

public partial class CefStreamWriter
{
    /// <summary>
    /// Creates a stream writer that writes to the specified file.
    /// </summary>
    public static ICefStreamWriter CreateForFile(string fileName)
    {
        return Cef.CreateStreamWriterForFile(fileName)!;
    }

    /// <summary>
    /// Creates a stream writer that writes to the provided handler.
    /// </summary>
    public static ICefStreamWriter CreateForHandler(ICefWriteHandler handler)
    {
        return Cef.CreateStreamWriterForHandler(handler)!;
    }
}
