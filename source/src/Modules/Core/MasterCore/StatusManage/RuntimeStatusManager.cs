using System;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Runtime;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.StatusManage
{
    /// <summary>
    /// 运行时所有测试的状态管理
    /// </summary>
    internal class RuntimeStatusManager : IMessageHandler, IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private Thread _internalMessageThd;
        private CancellationTokenSource _cancellation;

        public RuntimeStatusManager(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
        }

        public bool HandleMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            this._internalMessageThd = new Thread(ProcessInternalMessage)
            {
                Name = "InnerMessageListener",
                IsBackground = true
            };
            _internalMessageThd.Start();
        }

        private void ProcessInternalMessage(object state)
        {
            LocalEventQueue<EventInfoBase> internalEventQueue = _globalInfo.EventQueue;
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    EventInfoBase eventInfo = internalEventQueue.WaitUntilMessageCome();
                    InternalMessageProcessingLoop(eventInfo);
                }
            }
            catch (TestflowException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                    $"thread {Thread.CurrentThread.Name} is stopped abnormally");
            }
            catch (Exception ex)
            {
                _globalInfo.EventQueue.Enqueue(new ExceptionEventInfo(ex));
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
            }
        }

        private void InternalMessageProcessingLoop(EventInfoBase eventInfo)
        {
        }

        public void Stop()
        {
            _cancellation.Cancel();
            ModuleUtils.StopThreadWork(_internalMessageThd);
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}