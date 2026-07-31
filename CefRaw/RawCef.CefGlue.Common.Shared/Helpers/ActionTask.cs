using System;
using RawCef;
using RawCef.Native;

namespace Xilium.CefGlue.Common.Shared.Helpers
{
    internal sealed class ActionTask : CefTask
    {
        private Action _action;

        public ActionTask(Action action)
        {
            _action = action;
        }

        public override void Execute()
        {
            _action();
            _action = null;
        }

        public static void Run(Action action, CefThreadId threadId = CefThreadId.Ui)
        {
            Cef.PostTask(threadId, new ActionTask(action));
        }
    }
}
