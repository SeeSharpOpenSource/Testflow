using System.Collections.Generic;
using Testflow.Modules;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    internal class RuntimeDatabaseProxy : DatabaseProxy
    {
        public RuntimeDatabaseProxy(IModuleConfigData configData) : base(configData, true)
        {
        }

        public override int GetTestInstanceCount(string filterString)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<TestInstanceData> GetTestInstances(string filterString)
        {
            throw new System.InvalidOperationException();
        }
        
        public override void DeleteTestInstance(string fileterString)
        {
            throw new System.InvalidOperationException();
        }
    }
}