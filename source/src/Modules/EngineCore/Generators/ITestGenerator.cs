using Testflow.Data.Sequence;
using Testflow.EngineCore.Container;

namespace Testflow.EngineCore.Generators
{
    internal interface ITestGenerator
    {
        RuntimeContainer Generate(ITestProject testProject, params object[] param);

        RuntimeContainer Generate(ISequenceGroup sequenceGroup, params object[] param);
    }
}