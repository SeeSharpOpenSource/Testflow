using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Testflow.CoreCommon.Messages;

namespace Testflow.SlaveCore.Common
{
    internal class CallBackEventManager
    {
        /// <summary>
        /// int callBackId
        /// </summary>
        private IDictionary<int, AutoResetEvent> _blockerMapper;
        private static object _blockerLocker = new object();
        private IDictionary<int, CallBackMessage> _messageMapper;
        private static object _messageLocker = new object();
        private int _callBackId;

        internal CallBackEventManager()
        {
            _blockerMapper = new Dictionary<int, AutoResetEvent>(Constants.DefaultRuntimeSize);
            _messageMapper = new Dictionary<int, CallBackMessage>(Constants.DefaultRuntimeSize);
            _callBackId = 0;
        }

        //CallBackId全session唯一
        internal int GetCallBackId()
        {
            return Interlocked.Increment(ref _callBackId);
        }

        //同步：stepCallBackEntity调用来阻塞
        internal AutoResetEvent AcquireBlockEvent(int callBackId)
        {
            AutoResetEvent blocker = new AutoResetEvent(false);
            lock (_blockerLocker)
            {
                Thread.MemoryBarrier();
                _blockerMapper.Add(callBackId, blocker);
            }
            return blocker;
        }

        //同步：DownLinkMessageProcessor调用来告知message并取消阻塞
        internal void ReleaseBlock(CallBackMessage message)
        {
            lock (_blockerLocker)
            {
                Thread.MemoryBarrier();
                //判断slave是否已超过等待时间, _blockerMapper里就会没有键值对
                if (!_blockerMapper.ContainsKey(message.CallBackId))
                {
                    return;
                }
                lock (_messageLocker)
                {
                    _messageMapper.Add(message.CallBackId, message);
                }
                _blockerMapper[message.CallBackId].Set();
            }
        }

        //同步：如果不超时，slave正常调用此功能获得message信息并清理AutoResetEvent
        internal CallBackMessage GetMessageDisposeBlock(int callBackId)
        {
            lock (_blockerLocker)
            {
                Thread.MemoryBarrier();
                _blockerMapper[callBackId].Dispose();
            }
            lock (_messageLocker)
            {
                Thread.MemoryBarrier();
                return _messageMapper[callBackId];
            }
        }

        //同步：slave超时就调用此方法，把_blockerMapper里的键值对清理掉
        internal void TimeoutForceDispose(int callBackId)
        {
            lock (_blockerLocker)
            {
                Thread.MemoryBarrier();
                _blockerMapper[callBackId].Dispose();
                _blockerMapper.Remove(callBackId);
            }
        }
    }
}
