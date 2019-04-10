using System.Collections.Generic;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.MasterCore.EventData
{
    internal class DebugInformation : IDebugInformation
    {
        public ICallStack BreakPoint { get; }
        public IDictionary<IVariable, string> WatchDatas { get; }

        public DebugInformation(DebugEventInfo eventInfo, ISequenceFlowContainer parentSequenceData)
        {
            this.BreakPoint = eventInfo.BreakPoint;
            this.WatchDatas = new Dictionary<IVariable, string>(eventInfo.WatchData.Count);
            for (int i = 0; i < eventInfo.WatchData.Count; i++)
            {
                IVariable variable = CoreUtils.GetVariable(parentSequenceData, eventInfo.WatchData.Names[i]);
                WatchDatas.Add(variable, eventInfo.WatchData.Values[i]);
            }
        }
    }
}