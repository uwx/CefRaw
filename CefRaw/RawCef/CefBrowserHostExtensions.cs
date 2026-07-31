namespace RawCef;

public static class CefBrowserHostExtensions
{
    public static unsafe bool SendDevToolsMessage(this ICefBrowserHost browserHost, byte[] message)
    {
        fixed (byte* messagePtr = &message[0])
        {
            return browserHost.SendDevToolsMessage(messagePtr, (nuint)message.Length) != 0;
        }
    }

    public static unsafe bool SendDevToolsMessage(this ICefBrowserHost browserHost, ArraySegment<byte> message)
    {
        fixed (byte* messagePtr = &message.Array![message.Offset])
        {
            return browserHost.SendDevToolsMessage(messagePtr, (nuint)message.Count) != 0;
        }
    }
}