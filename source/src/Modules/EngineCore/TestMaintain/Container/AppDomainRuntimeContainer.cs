using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;

namespace Testflow.EngineCore.TestMaintain.Container
{
    internal class AppDomainRuntimeContainer : RuntimeContainer
    {
        public AppDomainRuntimeContainer(ISequenceFlowContainer sequence, ModuleGlobalInfo globalInfo,
            params object[] extraParam) : base(sequence, globalInfo)
        {

        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}