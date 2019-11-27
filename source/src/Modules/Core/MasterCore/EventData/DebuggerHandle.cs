using Testflow.MasterCore.Core;
using Testflow.Runtime;

namespace Testflow.MasterCore.EventData
{
    internal class DebuggerHandle : IDebuggerHandle
    {
        private readonly DebugManager _debugManager;
        public DebuggerHandle(DebugManager debugManager)
        {
            this._debugManager = debugManager;
        }

        public void StepInto()
        {
            _debugManager.StepInto();
        }

        public void StepOver()
        {
            _debugManager.StepOver();
        }

        public void Continue(int session)
        {
            _debugManager.Continue(session);
        }

        public void RunToEnd()
        {
            _debugManager.RunToEnd();
        }

        public void Pause(int session)
        {
            _debugManager.Pause(session);
        }
    }
}