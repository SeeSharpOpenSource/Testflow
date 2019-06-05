using System;
using System.Threading;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;

namespace Testflow.MasterCore
{
    /// <summary>
    /// 运行时引擎的对外接口
    /// </summary>
    public class EngineHandle : IEngineController
    {
        private static EngineHandle _instance;
        private static object _instLock = new object();
        private RuntimeEngine _runtimeEngine;

        public EngineHandle()
        {
            I18NOption i18NOption = new I18NOption(typeof (EngineHandle).Assembly, "i18n_engineCore_zh",
                "i18n_engineCore_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);

            if (null != _instance)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(CommonErrorCode.InternalError, i18N.GetStr("InstAlreadyExist"));
            }
            lock (_instLock)
            {
                Thread.MemoryBarrier();
                if (null != _instance)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowRuntimeException(CommonErrorCode.InternalError, i18N.GetStr("InstAlreadyExist"));
                }
                _instance = this;
            }
        }

        public IModuleConfigData ConfigData { get; set; }

        public void RuntimeInitialize()
        {
            _runtimeEngine = new RuntimeEngine(ConfigData);
            _runtimeEngine.GlobalInfo.ExceptionManager.ExceptionRaised += (exception) => { ExceptionRaised?.Invoke(exception); };
        }

        public void DesigntimeInitialize()
        {
            // ignore
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this.ConfigData = configData;
        }

        public RuntimeState State => _runtimeEngine?.GlobalInfo?.StateMachine.State ?? RuntimeState.NotAvailable;
        public event Action<Exception> ExceptionRaised;

        public void SetSequenceData(ISequenceFlowContainer sequenceData)
        {
            _runtimeEngine.Initialize(sequenceData);
        }

        public RuntimeState GetRuntimeState(int sessionId)
        {
            return _runtimeEngine.GetRuntimeState(sessionId);
        }

        public TDataType GetComponent<TDataType>(string componentName, params object[] extraParams)
        {
            return _runtimeEngine.GetComponent<TDataType>(componentName, extraParams);
        }

        public TDataType GetRuntimeInfo<TDataType>(string infoName, params object[] extraParams)
        {
            return _runtimeEngine.GetRuntimeInfo<TDataType>(infoName, extraParams);
        }

        public long AddRuntimeObject(string objectType, int sessionId, params object[] param)
        {
            return _runtimeEngine.AddRuntimeObject(objectType, sessionId, param);
        }

        public long RemoveRuntimeObject(int objectId, params object[] param)
        {
            return _runtimeEngine.RemoveRuntimeObject(objectId, param);
        }

        public void RegisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams)
        {
            _runtimeEngine.RegisterRuntimeEvent(callBack, eventName, extraParams);
        }

        public void UnregisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams)
        {
            _runtimeEngine.UnregisterRuntimeEvent(callBack, eventName, extraParams);
        }

        public void AbortRuntime(int sessionId)
        {
            _runtimeEngine.AbortRuntime(sessionId);
        }

        public void Start()
        {
            _runtimeEngine.Start();
        }

        public void Stop()
        {
            _runtimeEngine?.Stop();
        }

        public void Dispose()
        {
            _runtimeEngine?.Dispose();
            _instance = null;
        }
    }
}