using System;
using System.Threading;
using Testflow.Common;
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

        public EngineHandle()
        {
            I18NOption i18NOption = new I18NOption(typeof (EngineHandle).Assembly, "i18n_engineCore_zh.resx",
                "i18n_engineCore_en.resx")
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
            RuntimeEngine runtimeEngine = new RuntimeEngine(ConfigData);
        }

        public void DesigntimeInitialize()
        {
            // ignore
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this.ConfigData = configData;
        }

        public RuntimeState State { get; }

        public RuntimeState GetRuntimeState(int sessionId)
        {
            throw new NotImplementedException();
        }

        public TDataType GetComponent<TDataType>(string componentName, params object[] extraParams)
        {
            throw new NotImplementedException();
        }

        public TDataType GetRuntimeInfo<TDataType>(string infoName, params object[] extraParams)
        {
            throw new NotImplementedException();
        }

        public int AddRuntimeObject(string objectType, int sessionId, params object[] param)
        {
            throw new NotImplementedException();
        }

        public int RemoveRuntimeObject(int objectId, params object[] param)
        {
            throw new NotImplementedException();
        }

        public void RegisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams)
        {
            throw new NotImplementedException();
        }

        public void UnregisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams)
        {
            throw new NotImplementedException();
        }

        public int SetRuntimeTarget(ISequenceGroup sequenceGroup)
        {
            throw new NotImplementedException();
        }

        public int SetRuntimeTarget(ITestProject testProject)
        {
            throw new NotImplementedException();
        }

        public void AbortRuntime(int sessionId)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}