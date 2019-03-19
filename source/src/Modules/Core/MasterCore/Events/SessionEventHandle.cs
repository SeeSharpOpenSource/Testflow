using System;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.MasterCore.Events
{
    internal class SessionEventHandle
    {
        public SessionEventHandle(int sessionId)
        {
            this.SessionId = sessionId;
        }

        /// <summary>
        /// 测试生成开始事件
        /// </summary>
        public event RuntimeDelegate.TestGenerationAction TestGenerationStart;

        /// <summary>
        /// 测试生成中间事件，生成过程中会不间断生成该事件
        /// </summary>
        public event RuntimeDelegate.TestGenerationAction TestGenerationReport;

        /// <summary>
        /// 测试生成结束事件
        /// </summary>
        public event RuntimeDelegate.TestGenerationAction TestGenerationEnd;

        /// <summary>
        /// Events raised when a sequence is start and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.StatusReceivedAction SequenceStarted;

        /// <summary>
        /// Events raised when receive runtime status information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.StatusReceivedAction StatusReceived;

        /// <summary>
        /// Events raised when a sequence is over and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.StatusReceivedAction SequenceOver;

        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.TestSessionOverAction TestOver;

        /// <summary>
        /// 断点命中事件，当某个断点被命中时触发
        /// </summary>
        public event RuntimeDelegate.BreakPointHittedAction BreakPointHitted;

        //

        //        /// <summary>

        //        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.

        //        /// </summary>

        //        event RuntimeDelegate.StatusReceivedAction SequenceFailed;

        public int SessionId { get; }

        internal void OnTestGenerationStart(ITestGenerationInfo generationinfo)
        {
            TestGenerationStart?.Invoke(generationinfo);
        }

        internal void OnTestGenerationEnd(ITestGenerationInfo generationinfo)
        {
            TestGenerationEnd?.Invoke(generationinfo);
        }

        internal void OnSequenceStarted(IRuntimeStatusInfo statusinfo, ICallStack stack)
        {
            SequenceStarted?.Invoke(statusinfo, stack);
        }

        internal void OnStatusReceived(IRuntimeStatusInfo statusinfo, ICallStack stack)
        {
            StatusReceived?.Invoke(statusinfo, stack);
        }

        internal void OnSequenceOver(IRuntimeStatusInfo statusinfo, ICallStack stack)
        {
            SequenceOver?.Invoke(statusinfo, stack);
        }

        internal void OnTestOver(ITestResultCollection statistics, int sequencegroupIndex)
        {
            TestOver?.Invoke(statistics, sequencegroupIndex);
        }

        internal void OnBreakPointHitted(ISequenceDebugger debugger, IDebugInformation information)
        {
            BreakPointHitted?.Invoke(debugger, information);
        }

        internal void OnTestGenerationReport(ITestGenerationInfo generationinfo)
        {
            TestGenerationReport?.Invoke(generationinfo);
        }
    }
}