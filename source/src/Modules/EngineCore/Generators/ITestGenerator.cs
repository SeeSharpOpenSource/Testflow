using Testflow.Data.Sequence;
using Testflow.EngineCore.Container;
using Testflow.EngineCore.Message;

namespace Testflow.EngineCore.Generators
{
    internal interface ITestGenerator : IMessageConsumer
    {
        RuntimeContainer Generate(ITestProject testProject, params object[] param);

        RuntimeContainer Generate(ISequenceGroup sequenceGroup, params object[] param);
    }
}