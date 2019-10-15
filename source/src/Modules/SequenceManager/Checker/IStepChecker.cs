using Testflow.Data.Sequence;

namespace Testflow.SequenceManager.Checker
{
    internal interface IStepChecker
    {
        void CheckStep(ISequenceStep step);
    }
}