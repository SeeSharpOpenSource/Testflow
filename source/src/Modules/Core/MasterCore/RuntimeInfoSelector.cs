using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Core;
using Testflow.Usr;

namespace Testflow.MasterCore
{
    internal class RuntimeInfoSelector
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly RuntimeEngine _engine;

        public RuntimeInfoSelector(ModuleGlobalInfo globalInfo, RuntimeEngine runtimeEngine)
        {
            _globalInfo = globalInfo;
            _engine = runtimeEngine;
        }

        public object GetRuntimeInfo(string infoName, params object[] extraParams)
        {
            object infoValue = null;
            int session = 0;
            switch (infoName)
            {
                case Constants.RuntimeStateInfo:
                    infoValue = GetRuntimeState(extraParams);
                    break;
                case Constants.ElapsedTimeInfo:
                    infoValue = GetElapsedTime(extraParams);
                    break;
                case Constants.RuntimeHashInfo:
                    infoValue = GetRuntimeHash();
                    break;
                case Constants.TestInstanceName:
                    infoValue = GetTestName();
                    break;
                case Constants.DebugHanle:
                    infoValue = GetDebugHandle();
                    break;
                default:
                    _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                        $"Unsupported runtime object type: {0}.");
                    _globalInfo.ExceptionManager.Append(new TestflowDataException(
                        ModuleErrorCode.InvalidRuntimeInfoName,
                        _globalInfo.I18N.GetFStr("InvalidRuntimeInfoName", infoName)));
                    break;
            }
            return infoValue;
        }

        private object GetRuntimeState(object[] extraParams)
        {
            object infoValue = null;
            int session;
            if (extraParams.Length == 0)
            {
                infoValue = _globalInfo.StateMachine.State;
            }
            else if (extraParams.Length == 1)
            {
                session = (int) extraParams[0];
                infoValue = _engine.StatusManager[session].State;
            }
            else if (extraParams.Length == 2)
            {
                session = (int) extraParams[0];
                int sequenceIndex = (int) extraParams[1];
                infoValue = _engine.StatusManager[session][sequenceIndex].State;
            }
            return infoValue;
        }

        private object GetElapsedTime(object[] extraParams)
        {
            object infoValue = null;
            int session;
            if (extraParams.Length == 1)
            {
                session = (int) extraParams[0];
                infoValue = _engine.StatusManager[session].ElapsedTime.TotalMilliseconds;
            }
            else if (extraParams.Length == 2)
            {
                session = (int) extraParams[0];
                int sequenceIndex = (int) extraParams[1];
                infoValue = _engine.StatusManager[session][sequenceIndex].ElapsedTime.TotalMilliseconds;
            }
            return infoValue;
        }

        private object GetRuntimeHash()
        {
            return _globalInfo.RuntimeHash;
        }

        private object GetTestName()
        {
            return _globalInfo.ConfigData.GetProperty<string>("TestName");
        }

        private object GetDebugHandle()
        {
            return _globalInfo.DebugHandle;
        }
    }
}