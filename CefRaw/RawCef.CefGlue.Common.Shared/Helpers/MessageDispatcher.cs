using System;
using System.Collections.Generic;
using RawCef;
using RawCef.Native;

namespace Xilium.CefGlue.Common.Shared.Helpers
{
    internal class MessageDispatcher
    {
        private readonly Dictionary<string, Action<MessageReceivedEventArgs>> _messageHandlers = new Dictionary<string, Action<MessageReceivedEventArgs>>();

        public void DispatchMessage(ICefBrowser browser, ICefFrame frame, CefProcessId sourceProcess, ICefProcessMessage message)
        {
            if (_messageHandlers.TryGetValue(message.GetName()!, out var existingHandler))
            {
                existingHandler(new MessageReceivedEventArgs(browser, frame, sourceProcess, message));
            }
        }

        public void RegisterMessageHandler(string messageName, Action<MessageReceivedEventArgs> handler)
        {
            _messageHandlers.TryGetValue(messageName, out var existingHandler);
            _messageHandlers[messageName] = existingHandler + handler;
        }
    }
}
