using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.ObjectManage.Objects
{
    internal class BreakPointObject : RuntimeObject
    {
        public CallStack BreakPoint { get; }

        public BreakPointObject(CallStack breakPoint) : base(Constants.BreakPointObjectName)
        {
            this.BreakPoint = breakPoint;
        }
    }
}