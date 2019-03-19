using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.CoreCommon.Data
{
    public class EventQueue : IDisposable
    {
        private readonly Queue<MessageInfo> _events;
        private SpinLock _queueLock;

        public EventQueue()
        {
            _events = new Queue<MessageInfo>(Constants.DefaultEventsQueueSize);
            _queueLock = new SpinLock();
        }

        public void Enqueue(MessageInfo item)
        {
            bool getLock = GetLock();
            if (_events.Count >= Constants.MaxEventsQueueSize)
            {
                ReleaseLock(getLock);
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.EventsTooMany, i18N.GetStr("EventQueueOverflow"));
            }
            _events.Enqueue(item);
            ReleaseLock(getLock);
        }

        public MessageInfo Dequeue()
        {
            bool getLock = GetLock();
            MessageInfo item = null;
            if (_events.Count > 0)
            {
                item = _events.Dequeue();
            }
            ReleaseLock(getLock);
            return item;
        }

        private bool GetLock()
        {
            bool getLock = false;
            _queueLock.TryEnter(Constants.EventQueueTimeOut, ref getLock);
            if (!getLock)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.EventTimeOut, i18N.GetStr("EventEnqueueTimeOut"));
            }
            return true;
        }

        private void ReleaseLock(bool getLock)
        {
            if (getLock)
            {
                _queueLock.Exit();
            }
        }

        public void Dispose()
        {
            this._events.Clear();
        }
    }
}