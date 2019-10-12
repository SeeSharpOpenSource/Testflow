using System;
using System.Reflection;
using Testflow.Data.Sequence;

namespace Testflow.SequenceManager.StepCreators
{
    internal abstract class SequenceStepCreator
    {
        public static ISequenceStep CreateSequenceStep(SequenceStepType stepType)
        {
            string creatorName = $"Testflow.SequenceManager.StepCreators.{stepType}Creator";
            Type creatorType = Type.GetType(creatorName);
            ConstructorInfo constructor = creatorType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
            SequenceStepCreator creator = (SequenceStepCreator)constructor.Invoke(new object[0]);
            return creator.CreateSequenceStep();
        }

        public static void StepDataCheck(ISequenceStep step)
        {
            throw new NotImplementedException();
        }

        protected abstract ISequenceStep CreateSequenceStep();
    }
}