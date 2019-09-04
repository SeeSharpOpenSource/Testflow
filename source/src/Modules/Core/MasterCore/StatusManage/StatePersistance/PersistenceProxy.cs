using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.StatusManage.StatePersistance
{
    internal class PersistenceProxy
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private IDataMaintainer _dataMaintainer;

        public PersistenceProxy(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
            _dataMaintainer = globalInfo.TestflowRunner.DataMaintainer;
        }

        public void WriteData(TestInstanceData testInstance)
        {
            _dataMaintainer.AddData(testInstance);
        }

        public void WriteData(SessionResultData result)
        {
            _dataMaintainer.AddData(result);
        }

        public void WriteData(SequenceResultData result)
        {
            _dataMaintainer.AddData(result);
        }

        public void WriteData(RuntimeStatusData status)
        {
            _dataMaintainer.AddData(status);
        }

        public void WriteData(PerformanceStatus performance)
        {
            _dataMaintainer.AddData(performance);
        }

        public void UpdateData(TestInstanceData testInstance)
        {
            _dataMaintainer.UpdateData(testInstance);
        }

        public void UpdateData(SessionResultData result)
        {
            _dataMaintainer.UpdateData(result);
        }

        public void UpdateData(SequenceResultData result)
        {
            _dataMaintainer.UpdateData(result);
        }

        public IPerformanceResult GetPerformanceResult(int session)
        {
            PerformanceResult performanceResult = new PerformanceResult();
            _dataMaintainer.GetPerformanceResult(_globalInfo.RuntimeHash, session, performanceResult);
            return performanceResult;
        }

        public bool ExistFailedStep(int session, int sequence)
        {
            return _dataMaintainer.ExistFailedStep(_globalInfo.RuntimeHash, session, sequence);
        }
    }
}