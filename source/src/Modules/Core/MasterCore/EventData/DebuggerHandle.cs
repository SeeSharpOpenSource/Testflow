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

        public void Continue()
        {
            _debugManager.Continue();
        }

        public void RunToEnd()
        {
            _debugManager.RunToEnd();
        }

        public void Pause()
        {
            _debugManager.Pause();
        }
    }
}