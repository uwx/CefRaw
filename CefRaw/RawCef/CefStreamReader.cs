namespace RawCef;

public partial class CefStreamReader
{
    /// <summary>
    /// Creates a stream reader that reads from the specified file.
    /// </summary>
    public static ICefStreamReader CreateForFile(string fileName)
    {
        return Cef.CreateStreamReaderForFile(fileName)!;
    }

    /// <summary>
    /// Creates a stream reader that reads from the provided data.
    /// </summary>
    public static ICefStreamReader CreateForData(ReadOnlySpan<byte> data)
    {
        return Cef.CreateStreamReaderForData(data)!;
    }

    /// <summary>
    /// Creates a stream reader that reads from the provided handler.
    /// </summary>
    public static ICefStreamReader CreateForHandler(ICefReadHandler handler)
    {
        return Cef.CreateStreamReaderForHandler(handler)!;
    }
}
