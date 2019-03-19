using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.TestMaintain.Container
{
    internal class ProcessRuntimeContainer : RuntimeContainer
    {
        public ProcessRuntimeContainer(int session, ModuleGlobalInfo globalInfo,
            params object[] extraParam) : base(session, globalInfo)
        {
             // TODO
        }

        public override void Start(string startConfigData)
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}