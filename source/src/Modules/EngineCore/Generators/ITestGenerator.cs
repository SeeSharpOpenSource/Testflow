using Testflow.Data.Sequence;
using Testflow.EngineCore.RuntimeContainer;

namespace Testflow.EngineCore.Generators
{
    internal interface ITestGenerator
    {
        IRuntimeContainer Generate(ITestProject testProject, params object[] param);

        IRuntimeContainer Generate(ISequenceGroup sequenceGroup, params object[] param);
    }
}