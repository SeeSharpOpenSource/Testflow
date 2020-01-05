using System;
using Testflow.CoreCommon.Data;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Data
{
    internal class ExecutionTrack : IDisposable
    {
        private OverLapBuffer<StepExecutionInfo> _stepExecutionInfos;
        public ExecutionTrack(int capacity)
        {
            _stepExecutionInfos = new OverLapBuffer<StepExecutionInfo>(capacity);
        }

        public void Enqueue(StepTaskEntityBase stepEntity)
        {
            _stepExecutionInfos.Enqueue(new StepExecutionInfo(stepEntity, StepResult.NotAvailable));
        }

        public StepExecutionInfo GetLastStep(int offset)
        {
            return _stepExecutionInfos.GetLastElement(offset);
        }

        public StepExecutionInfo GetLastNotAvailableStep()
        {
            StepExecutionInfo stepInfo;
            int offset = 1;
            do
            {
                stepInfo = _stepExecutionInfos.GetLastElement(offset);
            } while (null != stepInfo && stepInfo.StepResult != StepResult.NotAvailable);
            return stepInfo;
        }

        public void Dispose()
        {
            _stepExecutionInfos.Dispose();
        }
    }
}