using System;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;

namespace Testflow.MasterCore.StatusManage
{
    internal class SessionStateMaintainer
    {
        private readonly ModuleGlobalInfo _globalInfo;

        public SessionStateMaintainer(ModuleGlobalInfo globalInfo, int session, ISequenceFlowContainer sequenceData)
        {
            this._globalInfo = globalInfo;
            this.Session = session;
            this.SequenceData = sequenceData;
        }

        public int Session { get; }
        
        public ISequenceFlowContainer SequenceData { get; }

        public RuntimeState State { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime StopTime { get; private set; }

        public void RecordEvent(MessageBase message)
        {
            StatusMessage statusMsg = (StatusMessage)message;
        }

        public void AbortEventProcess(AbortEventInfo eventInfo)
        {

        }

        public void DebugEventProcess(DebugEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        public void ExceptionEventProcess(ExceptionEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        public void SyncEventProcess(SyncEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        public void TestGenEventProcess(TestStateEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

    }
}