using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Usr;
using Testflow.Utility.Utils;

namespace Testflow.ComInterfaceManager
{
    internal class DescriptionDataTable : IDisposable
    {
        // 类型完整名称到类型数据的映射
        private Dictionary<string, ITypeData> _typeMapping;
        // 程序集名称到程序集描述信息的映射
        private IDictionary<string, ComInterfaceDescription> _descriptions;
        // 引用程序集名称到程序集信息对象的映射。这部分程序集是因为包含加载程序集使用类型时被动引入的。
        // 这里的值在调用GetClassDescrition方法时才会被更新
        private IDictionary<string, IAssemblyInfo> _refAssemblyInfos;
        private ReaderWriterLockSlim _lock;
        private int _nextComIndex;

        public DescriptionDataTable()
        {
            this._descriptions = new Dictionary<string, ComInterfaceDescription>(200);
            this._typeMapping = new Dictionary<string, ITypeData>(1000);
            _refAssemblyInfos = new Dictionary<string, IAssemblyInfo>(100);
            _lock = new ReaderWriterLockSlim();
            _nextComIndex = 0;
        }

        protected int NextComId => Interlocked.Increment(ref _nextComIndex);
        public bool Add(ComInterfaceDescription description)
        {
            bool addSuccess = false;
            _lock.EnterWriteLock();
            if (!_descriptions.ContainsKey(description.Assembly.AssemblyName))
            {
                _descriptions.Add(description.Assembly.AssemblyName, description);
                addSuccess = true;
            }
            if (_refAssemblyInfos.ContainsKey(description.Assembly.AssemblyName))
            {
                _refAssemblyInfos.Remove(description.Assembly.AssemblyName);
            }
            _lock.ExitWriteLock();
            
            ModuleUtils.SetComponentId(description, NextComId);

            return addSuccess;
        }

        public void Add(IAssemblyInfo assemblyInfo)
        {
            _lock.EnterWriteLock();
            if (!_descriptions.ContainsKey(assemblyInfo.AssemblyName) &&
                !_refAssemblyInfos.ContainsKey(assemblyInfo.AssemblyName))
            {
                assemblyInfo.Path = StringUtil.NormalizeFilePath(assemblyInfo.Path);
                _refAssemblyInfos.Add(assemblyInfo.AssemblyName, assemblyInfo);
            }
            _lock.ExitWriteLock();
        }
        
        public bool Contains(string assemblyName)
        {
            _lock.EnterReadLock();
            bool containsKey = _descriptions.ContainsKey(assemblyName);
            _lock.ExitReadLock();
            return containsKey;
        }

        public bool Contains(int componentId)
        {
            _lock.EnterReadLock();
            bool containsKey = _descriptions.Any(item => item.Value.ComponentId == componentId);
            _lock.ExitReadLock();
            return containsKey;
        }

        public ComInterfaceDescription Remove(string assemblyName)
        {
            ComInterfaceDescription description = null;
            _lock.EnterWriteLock();
            if (_descriptions.ContainsKey(assemblyName))
            {
                description = _descriptions[assemblyName];
                _descriptions.Remove(assemblyName);
            }
            _lock.ExitWriteLock();
            return description;
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            _descriptions.Clear();
            _typeMapping.Clear();
            _lock.ExitWriteLock();
        }

        public ComInterfaceDescription GetComDescription(string assemblyName)
        {
            ComInterfaceDescription description = null;
            _lock.EnterReadLock();

            if (_descriptions.ContainsKey(assemblyName))
            {
                description = _descriptions[assemblyName];
            }
            _lock.ExitReadLock();
            return description;
        }

        public IAssemblyInfo GetAssemblyInfo(string assemblyName)
        {
            IAssemblyInfo assemblyInfo = null;
            _lock.EnterReadLock();

            if (_descriptions.ContainsKey(assemblyName))
            {
                assemblyInfo = _descriptions[assemblyName].Assembly;
            }
            else if (_refAssemblyInfos.ContainsKey(assemblyName))
            {
                assemblyInfo = _refAssemblyInfos[assemblyName];
            }
            _lock.ExitReadLock();
            return assemblyInfo;
        }

        public ComInterfaceDescription GetComDescription(int componentId)
        {
            _lock.EnterReadLock();
            ComInterfaceDescription description =
                _descriptions.Values.FirstOrDefault(item => item.ComponentId == componentId);
            _lock.ExitReadLock();
            return description;
        }

        public ComInterfaceDescription GetComDescriptionByPath(string path)
        {
            _lock.EnterReadLock();
            ComInterfaceDescription description =
                _descriptions.Values.FirstOrDefault(item => item.Assembly.Path.Equals(path));
            _lock.ExitReadLock();
            return description;
        }

        public void RemoveAssembly(string assemblyName)
        {
            _lock.EnterWriteLock();
            if (_descriptions.ContainsKey(assemblyName))
            {
                ComInterfaceDescription description = _descriptions[assemblyName];
                _descriptions.Remove(assemblyName);
                foreach (ITypeData variableType in description.VariableTypes)
                {
                    _typeMapping.Remove(ModuleUtils.GetFullName(variableType));
                }
                foreach (IClassInterfaceDescription classDescription in description.Classes)
                {
                    _typeMapping.Remove(ModuleUtils.GetFullName(classDescription.ClassType));
                }
                if (_refAssemblyInfos.ContainsKey(assemblyName))
                {
                    _refAssemblyInfos.Remove(assemblyName);
                }
            }
            _lock.ExitWriteLock();
        }

        public bool ContainsType(string fullName)
        {
            return _typeMapping.ContainsKey(fullName);
        }

        public ITypeData GetTypeData(string fullName)
        {
            return _typeMapping[fullName];
        }

        public IList<IComInterfaceDescription> GetComponentDescriptions()
        {
            return new List<IComInterfaceDescription>(_descriptions.Values);
        }

        public IList<ITypeData> GetTypeDatas()
        {
            return new List<ITypeData>(_typeMapping.Values);
        }

        public void AddTypeData(string fullName, ITypeData typeData)
        {
            _lock.EnterWriteLock();
            _typeMapping.Add(fullName, typeData);
            _lock.ExitWriteLock();
        }

        private int _diposedFlag = 0;
        public void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            _lock.Dispose();
        }
    }
}