using Testflow.SlaveCore.Controller;

namespace Testflow.SlaveCore.Runner
{
    internal class SequentialTestRunner : TestRunnerBase
    {
        public SequentialTestRunner(SlaveController controller, SlaveContext context) : base(context)
        {
        }
    }
}