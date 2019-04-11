using Testflow.CoreCommon.Data;

namespace Testflow.MasterCore.ObjectManage.Objects
{
    internal class BreakPointObject : RuntimeObject
    {
        public CallStack BreakPoint { get; }

        public BreakPointObject(CallStack breakPoint)
        {
            this.BreakPoint = breakPoint;
        }
    }
}