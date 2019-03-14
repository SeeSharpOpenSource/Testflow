using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.Runtime;

namespace Testflow.EngineCore.Data
{
    public class CallStack : ICallStack, ISerializable
    {
        public CallStack()
        {
            SequenceGroupIndex = Constants.UnverifiedSequenceIndex;
            SequenceIndex = Constants.UnverifiedSequenceIndex;
            this.StepStack = new List<int>(Constants.DefaultRuntimeSize);
        }

        public int SequenceGroupIndex { get; internal set; }
        public int SequenceIndex { get; internal set; }

        public IList<int> StepStack { get; internal set; }

        public static CallStack GetStack(ISequenceStep step)
        {
            CallStack callStack = new CallStack();

            ISequence sequence = null;
            ISequenceGroup sequenceGroup = null;
            while (null != step)
            {
                callStack.StepStack.Add(step.Index);
                ISequenceFlowContainer parent = step.Parent;
                if (parent is ISequence)
                {
                    sequence = parent as ISequence;
                }
                step = parent as ISequenceStep;
            }
            if (null == sequence)
            {
                return callStack;
            }
            callStack.SequenceIndex = sequence.Index;
            sequenceGroup = sequence.Parent as ISequenceGroup;
            if (!(sequenceGroup?.Parent is ITestProject))
            {
                return callStack;
            }
            ITestProject testProject = (ITestProject) sequenceGroup.Parent;
            callStack.SequenceGroupIndex = testProject.SequenceGroups.IndexOf(sequenceGroup);
            return callStack;
        }

        public CallStack(SerializationInfo info, StreamingContext context)
        {
            this.SequenceGroupIndex = (int) info.GetValue("SequenceGroupIndex", typeof(int));
            this.SequenceIndex = (int) info.GetValue("SequenceIndex", typeof(int));
            this.StepStack = info.GetValue("StepStack", typeof(List<int>)) as List<int>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SequenceGroupIndex", SequenceGroupIndex);
            info.AddValue("SequenceIndex", SequenceIndex);
            info.AddValue("StepStack", StepStack, typeof(List<int>));
        }
    }
}