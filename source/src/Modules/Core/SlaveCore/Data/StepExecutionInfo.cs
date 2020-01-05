using Testflow.Runtime.Data;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Data
{
    internal class StepExecutionInfo
    {
        public StepResult StepResult { get; set; }

        public StepTaskEntityBase StepEntity { get; }

        public StepExecutionInfo(StepTaskEntityBase stepEntity, StepResult result)
        {
            this.StepEntity = stepEntity;
            this.StepResult = result;
        }
    }
}