using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ComInterfaceManager
{
    public class InterfaceManager : IComInterfaceManager
    {
        private DescriptionDataTable _descriptionData;
        private DescriptionLoaderManager _loaderManager;
        private IModuleConfigData _configData;

        public InterfaceManager()
        {
            I18NOption i18NOption = new I18NOption(typeof(InterfaceManager).Assembly, "i18n_commanager_zh", "i18n_commanager_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
        }

        public IModuleConfigData ConfigData { get; set; }
        public void RuntimeInitialize()
        {
            throw new InvalidOperationException();
        }

        public void DesigntimeInitialize()
        {
            _descriptionData?.Dispose();
            _loaderManager?.Dispose();

            _descriptionData = new DescriptionDataTable();
            _loaderManager = new DescriptionLoaderManager();
            _loaderManager.LoadDefaultAssemblyDescription(_descriptionData,
                _configData.GetProperty<string>("TestflowHome"));
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this._configData = configData;
        }

        public IComInterfaceDescription GetComInterfaceByName(string assemblyName)
        {
            return _descriptionData.GetComDescription(assemblyName);
        }

        public ITypeData GetTypeByName(string typename, string namespaceStr)
        {
            string fullName = ModuleUtils.GetFullName(namespaceStr, typename);
            return _descriptionData.ContainsType(fullName) ? _descriptionData.GetTypeData(fullName) : null;
        }

        public IComInterfaceDescription GetComInterfaceById(int componentId)
        {
            return _descriptionData.GetComDescription(componentId);
        }

        public IComInterfaceDescription GetComponentInterface(string path)
        {
            ComInterfaceDescription description = _descriptionData.GetComDescriptionByPath(path);
            if (null == description)
            {
                description = _loaderManager.LoadAssemblyDescription(path, _descriptionData);
            }
            return description;
        }

        public IComInterfaceDescription GetComponentInterface(IAssemblyInfo assemblyInfo)
        {
            ComInterfaceDescription description = _descriptionData.GetComDescription(assemblyInfo.AssemblyName);
            if (null == description)
            {
                description = _loaderManager.LoadAssemblyDescription(assemblyInfo, _descriptionData);
            }
            return description;
        }

        public IList<IComInterfaceDescription> GetComponentInterfaces(IList<string> paths)
        {
            List<IComInterfaceDescription> descriptions = new List<IComInterfaceDescription>(paths.Count);
            foreach (string path in paths)
            {
                descriptions.Add(GetComponentInterface(path));
            }
            return descriptions;
        }

        public IList<IComInterfaceDescription> GetComponentInterfaces(IAssemblyInfoCollection assemblyInfos)
        {
            List<IComInterfaceDescription> descriptions = new List<IComInterfaceDescription>(assemblyInfos.Count);
            foreach (IAssemblyInfo assemblyInfo in assemblyInfos)
            {
                descriptions.Add(GetComponentInterface(assemblyInfo));
            }
            return descriptions;
        }

        public IList<IComInterfaceDescription> GetComponentInterfaces(string directory)
        {
            if (!Directory.Exists(directory))
            {
                TestflowRunner.GetInstance().LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                    $"Directory '{directory}' does not exist.");
                return new List<IComInterfaceDescription>(0);
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            FileInfo[] fileInfos = directoryInfo.GetFiles(Constants.LibFileFilter);
            List<IComInterfaceDescription> descriptions = new List<IComInterfaceDescription>(fileInfos.Length);
            foreach (FileInfo fileInfo in fileInfos)
            {
                descriptions.Add(GetComponentInterface(fileInfo.FullName));
            }
            return descriptions;
        }

        public ITypeData GetPropertyType(ITypeData variableType, string propertyName)
        {
            return _loaderManager.GetPropertyType(variableType, propertyName, _descriptionData);
        }

        public IList<string> GetTypeProperties(ITypeData type, ITypeData propertyType = null)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetEnumItems(ITypeData typeData)
        {
            ComInterfaceDescription interfaceDescription = _descriptionData.GetComDescription(typeData.Name);
            string fullName = ModuleUtils.GetFullName(typeData);
            if (null != interfaceDescription)
            {
                return interfaceDescription.Enumerations.ContainsKey(fullName)
                    ? interfaceDescription.Enumerations[fullName]
                    : new string[0];
            }
            return _loaderManager.GetEnumItemsByType(typeData);
        }

        public IClassInterfaceDescription GetClassDescriptionByType(ITypeData typeData, out string path, out string version)
        {
            ComInterfaceDescription interfaceDescription = _descriptionData.GetComDescription(typeData.Name);
            IClassInterfaceDescription classDescription = null;
            // 如果该类型描述已存在则直接返回
            if (interfaceDescription != null &&
                null != (classDescription = interfaceDescription.Classes.FirstOrDefault(item => item.ClassType.Equals(typeData))))
            {
                path = interfaceDescription?.Assembly.Path ?? string.Empty;
                version = interfaceDescription?.Assembly.Version ?? Constants.DefaultVersion;
                return classDescription;
            }
            return _loaderManager.GetClassDescription(typeData, out path, out version);
        }

        public IList<IComInterfaceDescription> GetComponentDescriptions()
        {
            return _descriptionData.GetComponentDescriptions();
        }

        public IList<ITypeData> GetTypeDatas()
        {
            return _descriptionData.GetTypeDatas();
        }

        public void Dispose()
        {
            _descriptionData?.Dispose();
            _loaderManager?.Dispose();
            I18N.RemoveInstance(Constants.I18nName);
        }
    }
}