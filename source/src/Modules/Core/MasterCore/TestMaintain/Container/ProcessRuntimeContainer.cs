using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.TestMaintain.Container
{
    internal class ProcessRuntimeContainer : RuntimeContainer
    {
        public ProcessRuntimeContainer(ISequenceFlowContainer sequence, ModuleGlobalInfo globalInfo,
            params object[] extraParam) : base(sequence, globalInfo)
        {
             // TODO
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