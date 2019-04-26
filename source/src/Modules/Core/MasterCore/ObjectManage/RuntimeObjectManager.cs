using System.Collections.Generic;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.ObjectManage
{
    internal class RuntimeObjectManager
    {
        private readonly Dictionary<long, RuntimeObject> _runtimeObjects;
        private readonly Dictionary<string, IRuntimeObjectCustomer> _customers;

        public RuntimeObjectManager()
        {
            this._runtimeObjects = new Dictionary<long, RuntimeObject>(100);
            this._customers = new Dictionary<string, IRuntimeObjectCustomer>(Constants.DefaultRuntimeSize);
        }

        public void AddObject(RuntimeObject runtimeObject)
        {
            _runtimeObjects.Add(runtimeObject.Id, runtimeObject);
            _customers[runtimeObject.GetType().Name].AddObject(runtimeObject);
        }

        public void RemoveObject(long id)
        {
            if (!_runtimeObjects.ContainsKey(id))
            {
                return;
            }
            RuntimeObject runtimeObject = _runtimeObjects[id];
            _runtimeObjects.Remove(id);
            _customers[runtimeObject.GetType().Name].RemoveObject(runtimeObject);
        }

        public void RegisterCustomer<TCustomType>(IRuntimeObjectCustomer customer)
        {
            _customers.Add(typeof (TCustomType).Name, customer);
        }

        public RuntimeObject this[long id] => _runtimeObjects.ContainsKey(id) ? _runtimeObjects[id] : null;

        public void Clear()
        {
            foreach (long id in _runtimeObjects.Keys)
            {
                RemoveObject(id);
            }
        }
    }
}