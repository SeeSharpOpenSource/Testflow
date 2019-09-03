using System.Collections.Generic;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Data;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.ObjectManage.Objects;
using Testflow.Usr;

namespace Testflow.MasterCore.ObjectManage
{
    internal class RuntimeObjectManager
    {
        private readonly Dictionary<long, RuntimeObject> _runtimeObjects;
        private readonly Dictionary<string, IRuntimeObjectCustomer> _customers;
        private readonly ModuleGlobalInfo _globalInfo;

        public RuntimeObjectManager(ModuleGlobalInfo globalInfo)
        {
            this._runtimeObjects = new Dictionary<long, RuntimeObject>(100);
            this._customers = new Dictionary<string, IRuntimeObjectCustomer>(Constants.DefaultRuntimeSize);
            this._globalInfo = globalInfo;
        }

        public long AddRuntimeObject(string objectType, int sessionId, params object[] param)
        {
            RuntimeObject runtimeObject = null;
            switch (objectType)
            {
                case Constants.BreakPointObjectName:
                    runtimeObject = new BreakPointObject((CallStack)param[0]);
                    break;
                case Constants.WatchDataObjectName:
                    runtimeObject = new WatchDataObject(sessionId, (int)param[0],
                        (string)param[1]);
                    break;
                case Constants.EvaluationObjectName:
                    runtimeObject = new EvaluationObject(sessionId, (int)param[0],
                        (string)param[1]);
                    break;
                default:
                    _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                        $"Unsupported runtime object type: {0}.");
                    _globalInfo.ExceptionManager.Append(new TestflowDataException(
                        ModuleErrorCode.InvalidRuntimeObjectType,
                        _globalInfo.I18N.GetFStr("InvalidRuntimeObjType", objectType)));
                    return Constants.InvalidObjectId;
                    break;
            }
            AddObject(runtimeObject);
            return runtimeObject.Id;
        }

        public long RemoveRuntimeObject(int objectId, params object[] param)
        {
            if (null == this[objectId])
            {
                return Constants.InvalidObjectId;
            }
            RemoveObject(objectId);
            return objectId;
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