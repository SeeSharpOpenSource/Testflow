using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.EventData
{
    public class CoroutineStatusInfo : ICoroutineStatusInfo
    {
        public int CoroutineId { get; set; }
        public ICallStack Stack { get; set; }
        public StepResult Result { get; set; }
        public IFailedInfo FailedInfo { get; set; }
        public IDictionary<IVariable, string> WatchDatas { get; set; }
    }
}