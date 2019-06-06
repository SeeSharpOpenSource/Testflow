using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Usr;

namespace Testflow.ComInterfaceManager
{
    internal class DescriptionDataTable : IDisposable
    {
        private Dictionary<string, ITypeData> _typeMapping;
        private IDictionary<string, ComInterfaceDescription> _descriptions;
        private ReaderWriterLockSlim _lock;
        private int _nextComIndex;

        public DescriptionDataTable()
        {
            this._descriptions = new Dictionary<string, ComInterfaceDescription>(200);
            this._typeMapping = new Dictionary<string, ITypeData>(1000);
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
            _lock.ExitWriteLock();
            
            ModuleUtils.SetComponentId(description, _nextComIndex);

            return addSuccess;
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
        
        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}