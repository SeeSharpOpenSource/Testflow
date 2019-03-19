using Testflow.MasterCore.Data;

namespace Testflow.RemoteRunner.Data
{
    internal class BreakPoint
    {
        private readonly int _objectId;
        private readonly CallStack _callStack;

        public BreakPoint(int objectId, CallStack stack)
        {
            this._objectId = objectId;
            this._callStack = stack;
        }

        public override bool Equals(object obj)
        {
            CallStack stack = obj as CallStack;
            if (null == stack)
            {
                return false;
            }

            return stack.Equals(_callStack);
        }

        public bool HitBreakPoint(CallStack currentStack)
        {
            return currentStack.Equals(_callStack);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_objectId*397) ^ (_callStack != null ? _callStack.GetHashCode() : 0);
            }
        }
    }
}