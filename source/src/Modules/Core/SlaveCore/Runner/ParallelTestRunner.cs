using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Runner
{
    internal class ParallelTestRunner : TestRunner
    {
        public ParallelTestRunner(SlaveContext context) : base(context)
        {
        }

        public override void Start(SessionTaskEntity sessionExecutionModel)
        {
            throw new System.NotImplementedException();
        }
    }
}