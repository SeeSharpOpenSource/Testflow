using Testflow.Data.Sequence;
using Testflow.EngineCore.Data;
using Testflow.EngineCore.Events;

namespace Testflow.EngineCore
{
    internal class RuntimeEngine
    {
        private readonly EventsDispatcher _eventsDispatcher;

        public RuntimeEngine()
        {
            EventQueue eventQueue = new EventQueue();
            this._eventsDispatcher = new EventsDispatcher(eventQueue);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            
        }

        public void Clear()
        {
            _eventsDispatcher.Clear();
        }

        public void Start()
        {
        }

        public void Stop()
        {
            
        }

    }
}