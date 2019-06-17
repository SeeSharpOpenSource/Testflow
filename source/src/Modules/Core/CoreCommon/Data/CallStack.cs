using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Data
{
    public class CallStack : ICallStack, ISerializable
    {
        private const string StackDelim = "_";

        public CallStack()
        {
            Session = CoreConstants.UnverifiedSequenceIndex;
            Sequence = CoreConstants.UnverifiedSequenceIndex;
            this.StepStack = new List<int>(CoreConstants.DefaultRuntimeSize);
        }

        public int Session { get; internal set; }
        public int Sequence { get; internal set; }

        public IList<int> StepStack { get; internal set; }

        public static CallStack GetStack(int session, ISequenceStep step)
        {
            CallStack callStack = new CallStack();
            while (null != step)
            {
                callStack.StepStack.Insert(0, step.Index);
                ISequenceFlowContainer parent = step.Parent;
                if (parent is ISequence)
                {
                    callStack.Sequence = ((ISequence)parent).Index;
                    break;
                }
                step = parent as ISequenceStep;
            }
            callStack.Session = session;
            return callStack;
        }

        public static CallStack GetEmptyStack(int session, int sequenceIndex)
        {
            CallStack callStack = new CallStack()
            {
                Session = session,
                Sequence = sequenceIndex
            };
            callStack.StepStack.Add(CoreConstants.EmptyStepIndex);
            return callStack;
        }

        public static CallStack GetStack(string stackStr)
        {
            CallStack callStack = new CallStack();
            string[] stackElement = stackStr.Split(StackDelim.ToCharArray());
            callStack.Session = int.Parse(stackElement[0]);
            callStack.Sequence = int.Parse(stackElement[1]);
            for (int i = 2; i < stackElement.Length; i++)
            {
                callStack.StepStack.Add(int.Parse(stackElement[i]));
            }
            return callStack;
        }

        public CallStack(SerializationInfo info, StreamingContext context)
        {
            this.Session = (int) info.GetValue("Session", typeof(int));
            this.Sequence = (int) info.GetValue("Sequence", typeof(int));
            this.StepStack = info.GetValue("StepStack", typeof(List<int>)) as List<int>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Session", Session);
            info.AddValue("Sequence", Sequence);
            info.AddValue("StepStack", StepStack, typeof(List<int>));
        }

        public override bool Equals(object obj)
        {
            CallStack callStack = obj as CallStack;
            if (null == callStack || Session != callStack.Session ||
                Sequence != callStack.Sequence || callStack.StepStack.Count != StepStack.Count)
            {
                return false;
            }
            for (int i = 0; i < StepStack.Count; i++)
            {
                if (StepStack[i] != callStack.StepStack[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Session;
                hashCode = (hashCode * 397) ^ Sequence;
                hashCode = (hashCode * 397) ^ (StepStack != null ? StepStack.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            StringBuilder callStackStr = new StringBuilder(100);
            callStackStr.Append(Session)
                .Append(StackDelim)
                .Append(Sequence)
                .Append(StackDelim)
                .Append(string.Join(StackDelim, StepStack));
            return callStackStr.ToString();
        }
    }
}