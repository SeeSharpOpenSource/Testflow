using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.StatusManage.StatePersistance
{
    internal class PersistenceProxy
    {
        private readonly ModuleGlobalInfo _globalInfo;
        public PersistenceProxy(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
        }

        public void WriteData(TestInstanceData testInstance)
        {
            // TODO
        }

        public void WriteData(SessionResultData result)
        {
            // TODO
        }

        public void WriteData(SequenceResultData result)
        {
            // TODO
        }

        public void WriteData(RuntimeStatusData status)
        {
            // TODO
        }

        public void WriteData(PerformanceStatus performance)
        {
            // TODO
        }

        public void UpdateData(TestInstanceData testInstance)
        {
            // TODO
        }

        public void UpdateData(SessionResultData result)
        {
            // TODO
        }

        public void UpdateData(SequenceResultData result)
        {
            // TODO
        }
    }
}