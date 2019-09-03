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
            throw new System.NotImplementedException();
        }

        public void StepOver()
        {
            throw new System.NotImplementedException();
        }

        public void Continue()
        {
            _debugManager.Continue();
        }

        public void RunToEnd()
        {
            throw new System.NotImplementedException();
        }
    }
}