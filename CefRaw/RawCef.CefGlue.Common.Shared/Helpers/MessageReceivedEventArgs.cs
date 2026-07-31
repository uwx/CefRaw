using RawCef;
using RawCef.Native;

namespace Xilium.CefGlue.Common.Shared.Helpers
{
    internal class MessageReceivedEventArgs
    {
        public MessageReceivedEventArgs(ICefBrowser browser, ICefFrame frame, CefProcessId processId, ICefProcessMessage message)
        {
            Browser = browser;
            Frame = frame;
            ProcessId = processId;
            Message = message;
        }

        public ICefBrowser Browser { get; }
        public ICefFrame Frame { get; }
        public CefProcessId ProcessId { get; }
        public ICefProcessMessage Message { get; }
    }
}
