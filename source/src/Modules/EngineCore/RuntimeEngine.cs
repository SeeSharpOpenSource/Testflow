using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.EngineCore.Data;
using Testflow.EngineCore.Events;
using Testflow.EngineCore.Message;
using Testflow.Modules;

namespace Testflow.EngineCore
{
    internal class RuntimeEngine
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly MessageTransceiver _messageTransceiver;

        public RuntimeEngine(IModuleConfigData configData)
        {
            _globalInfo = new ModuleGlobalInfo(configData);
            bool isSyncMessenger = _globalInfo.ConfigData.GetProperty<bool>("EngineSyncMessenger");
            _messageTransceiver = MessageTransceiver.GetTransceiver(_globalInfo, isSyncMessenger);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            
        }

        public void Clear()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {
            
        }

    }
}