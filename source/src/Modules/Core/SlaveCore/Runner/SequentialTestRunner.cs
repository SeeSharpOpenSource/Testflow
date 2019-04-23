using System.Threading;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Runner
{
    internal class SequentialTestRunner : TestRunner
    {
        

        public SequentialTestRunner(SlaveContext context) : base(context)
        {
        }

        public override void Start(SessionTaskEntity sessionExecutionModel)
        {
            
        }
    }
}