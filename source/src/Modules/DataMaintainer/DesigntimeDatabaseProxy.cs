using Testflow.Modules;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    internal class DesigntimeDatabaseProxy : DatabaseProxy
    {
        public DesigntimeDatabaseProxy(IModuleConfigData configData) : base(configData, false)
        {
        }

        public override void AddData(TestInstanceData testInstance)
        {
            throw new System.InvalidOperationException();
        }

        public override void UpdateData(TestInstanceData testInstance)
        {
            throw new System.InvalidOperationException();
        }

        public override void AddData(SessionResultData sessionResult)
        {
            throw new System.InvalidOperationException();
        }

        public override void UpdateData(SessionResultData sessionResult)
        {
            throw new System.InvalidOperationException();
        }

        public override void AddData(SequenceResultData sequenceResult)
        {
            throw new System.InvalidOperationException();
        }

        public override void UpdateData(SequenceResultData sequenceResult)
        {
            throw new System.InvalidOperationException();
        }

        public override void AddData(PerformanceStatus performanceStatus)
        {
            throw new System.InvalidOperationException();
        }

        public override void AddData(RuntimeStatusData runtimeStatus)
        {
            throw new System.InvalidOperationException();
        }
    }
}