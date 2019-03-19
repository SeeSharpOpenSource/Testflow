using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.TestMaintain.Container
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