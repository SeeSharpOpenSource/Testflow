using System;
using System.Collections.Generic;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;

namespace Testflow.ComInterfaceManager
{
    public class InterfaceManager : IComInterfaceManager
    {
        private DescriptionCollections _descriptions;
        private DescriptionLoaderManager _loaderManager;
        private IModuleConfigData _configData;

        public InterfaceManager()
        {
            
        }
        
        public IModuleConfigData ConfigData { get; set; }
        public void RuntimeInitialize()
        {
            throw new InvalidOperationException();
        }

        public void DesigntimeInitialize()
        {
            _descriptions = new DescriptionCollections();
            _loaderManager = new DescriptionLoaderManager();
            
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this._configData = configData;
        }

        public IComInterfaceDescription GetComInterfaceByName(string assemblyName)
        {
            return _descriptions[assemblyName];
        }

        public ITypeData GetTypeByName(string typename, string namespaceStr)
        {
            throw new System.NotImplementedException();
        }

        public IComInterfaceDescription GetComInterfaceById(int componentId)
        {
            throw new System.NotImplementedException();
        }

        public IComInterfaceDescription GetComponentInterface(string path)
        {
            throw new System.NotImplementedException();
        }

        public IComInterfaceDescription GetComponentInterface(IAssemblyInfo assemblyInfo)
        {
            throw new System.NotImplementedException();
        }

        public IList<IComInterfaceDescription> GetComponentInterfaces(IList<string> paths)
        {
            throw new System.NotImplementedException();
        }

        public IList<IComInterfaceDescription> GetComponentInterfaces(IAssemblyInfoCollection assemblyInfos)
        {
            throw new System.NotImplementedException();
        }

        public IList<IComInterfaceDescription> GetComponentInterfaces(string directory)
        {
            throw new System.NotImplementedException();
        }

        public ITypeData GetPropertyType(ITypeData variableType, string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public IList<string> GetTypeProperties(ITypeData type, ITypeData propertyType = null)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            _descriptions?.Dispose();
            _loaderManager?.Dispose();
        }
    }
}