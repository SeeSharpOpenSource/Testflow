using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Coroutine;
using Testflow.SlaveCore.Runner.Model;
using Testflow.Usr;

namespace Testflow.SlaveCore.Debugger
{
    internal class DebugManager : IDisposable
    {
        private readonly SlaveContext _context;

        // 各个声明的断点从stack字符串到StepEntity的映射
        private readonly Dictionary<string, StepTaskEntityBase> _breakPoints;

        private DebugWatchData _watchDatas;

        // 各个当前命中的断点，从Coroutine id到StepEntity的映射
        private readonly Dictionary<int, StepTaskEntityBase> _hitBreakPoints;

        private readonly ReaderWriterLockSlim _hitBreakPointsLock;

        public DebugManager(SlaveContext context)
        {
            _context = context;
            _watchDatas = new DebugWatchData();
            _breakPoints = new Dictionary<string, StepTaskEntityBase>(Constants.DefaultRuntimeSize);
            _hitBreakPoints = new Dictionary<int, StepTaskEntityBase>(20);
            _hitBreakPointsLock = new ReaderWriterLockSlim();
        }

        public void HandleDebugMessage(DebugMessage message)
        {
            switch (message.Name)
            {
                case MessageNames.AddBreakPointName:
                    AddBreakPoints(message.BreakPoints);
                    break;
                case MessageNames.DelBreakPointName:
                    DelBreakPoints(message.BreakPoints);
                    break;
                case MessageNames.PauseName:
                    // TODO 暂时写死Coroutine为0
                    Pause(0);
                    break;
                case MessageNames.StepOverName:
                    break;
                case MessageNames.ContinueName:
                    // TODO 暂时写死Coroutine为0
                    Continue(0);
                    break;
                case MessageNames.RunToEndName:
                    break;
                case MessageNames.RequestValueName:
                    break;
                case MessageNames.RefreshWatchName:
                    RefreshWatchData(message);
                    break;
                case MessageNames.StepIntoName:
                    break;
            }

            if (null != message.BreakPoints && message.BreakPoints.Count > 0)
            {
                AddBreakPoints(message.BreakPoints);
            }
        }

        private void AddBreakPoints(IList<CallStack> breakPoints)
        {
            foreach (CallStack breakPoint in breakPoints)
            {
                string stackStr = breakPoint.ToString();
                if (_breakPoints.ContainsKey(stackStr))
                {
                    continue;
                }
                StepTaskEntityBase stepEntity = ModuleUtils.GetStepEntity(_context, breakPoint);
                if (null == stepEntity)
                {
                    continue;
                }
                _breakPoints.Add(stackStr, stepEntity);
                stepEntity.PostListener += DebugBlocked;
            }
        }

        private void DelBreakPoints(List<CallStack> breakPoints)
        {
            foreach (CallStack breakPoint in breakPoints)
            {
                string stackStr = breakPoint.ToString();
                if (!_breakPoints.ContainsKey(stackStr))
                {
                    continue;
                }
                _breakPoints[stackStr].PostListener -= DebugBlocked;
                _breakPoints.Remove(stackStr);
            }
        }

        private void Pause(int coroutineId)
        {
            CoroutineHandle coroutineHandle = _context.CoroutineManager.GetCoroutineHandle(coroutineId);
            _hitBreakPointsLock.EnterWriteLock();

            if (_hitBreakPoints.ContainsKey(coroutineHandle.Id))
            {
                _hitBreakPointsLock.ExitWriteLock();
                return;
            }
            _hitBreakPoints.Add(coroutineHandle.Id, null);
            coroutineHandle.PostListener += DebugBlocked;

            _hitBreakPointsLock.ExitWriteLock();
        }

        private void Continue(int coroutineId)
        {
            _hitBreakPointsLock.EnterWriteLock();

            if (!_hitBreakPoints.ContainsKey(coroutineId))
            {
                _hitBreakPointsLock.ExitWriteLock();
                return;
            }
            CoroutineHandle coroutineHandle = _context.CoroutineManager.GetCoroutineHandle(coroutineId);
            coroutineHandle.PostListener -= DebugBlocked;
            _hitBreakPoints.Remove(coroutineHandle.Id);
            // 释放阻塞的协程
            coroutineHandle.SetSignal();
            _hitBreakPointsLock.ExitWriteLock();
        }

        private void StepInto()
        {
            // TODO
        }

        private void StepOver()
        {
            // TODO
        }

        private void RunToEnd()
        {
            // TODO
        }

        private void RefreshWatchData(DebugMessage message)
        {
            if (message.WatchData?.Names != null)
            {
                _watchDatas = message.WatchData;
                _hitBreakPointsLock.EnterReadLock();
                try
                {
                    // 如果存在Coroutine被阻塞，则在所有的断点上发送DebugMessage以更新当前节点的值
                    if (_hitBreakPoints.Count > 0)
                    {
                        _watchDatas.Values.Clear();
                        foreach (string watchData in _watchDatas.Names)
                        {
                            string variableName = ModuleUtils.GetVariableNameFromParamValue(watchData);
                            _watchDatas.Values.Add(_context.VariableMapper.GetWatchDataValue(variableName, watchData));
                        }
                        foreach (StepTaskEntityBase blockStep in _hitBreakPoints.Values)
                        {
                            if (null == blockStep)
                            {
                               continue;
                            }
                            CallStack breakPoint = blockStep.GetStack();
                            DebugMessage debugMessage = new DebugMessage(MessageNames.BreakPointHitName, _context.SessionId,
                                breakPoint, false)
                            {
                                WatchData = _watchDatas
                            };
                            _context.MessageTransceiver.SendMessage(debugMessage);
                        }
                        _context.LogSession.Print(LogLevel.Debug, _context.SessionId, "Refresh Watch data values.");
                    }
                }
                finally
                {
                    _hitBreakPointsLock.ExitReadLock();
                }
            }
            else
            {
                _watchDatas.Names.Clear();
                _watchDatas.Values.Clear();
            }
        }

        #region 断点事件

        private void DebugBlocked(StepTaskEntityBase stepTaskEntity)
        {
            _hitBreakPointsLock.EnterWriteLock();

            // 暂停的场景
            CoroutineHandle coroutineHandle = stepTaskEntity.Coroutine;
            if (_hitBreakPoints.ContainsKey(coroutineHandle.Id))
            {
                _hitBreakPoints[coroutineHandle.Id] = stepTaskEntity;
            }
            // 断点命中的场景
            else
            {
                _hitBreakPoints.Add(coroutineHandle.Id, stepTaskEntity);
            }
            _hitBreakPointsLock.ExitWriteLock();

            CallStack breakPoint = stepTaskEntity.GetStack();

            _watchDatas.Values.Clear();
            foreach (string watchData in _watchDatas.Names)
            {
                string variableName = ModuleUtils.GetVariableNameFromParamValue(watchData);
                _watchDatas.Values.Add(_context.VariableMapper.GetWatchDataValue(variableName, watchData));
            }
            DebugMessage debugMessage = new DebugMessage(MessageNames.BreakPointHitName, _context.SessionId,
                breakPoint, false)
            {
                WatchData = _watchDatas
            };

            // 发送断点命中消息
            _context.MessageTransceiver.SendMessage(debugMessage);
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, $"Breakpoint hitted:{breakPoint}");

            coroutineHandle.WaitSignal();
        }

        #endregion

        private int _diposedFlag = 0;
        public void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            _breakPoints.Clear();
            foreach (StepTaskEntityBase stepTaskEntity in _breakPoints.Values)
            {
                stepTaskEntity.PostListener -= DebugBlocked;
            }
            _breakPoints.Clear();

            _hitBreakPointsLock.EnterWriteLock();

            foreach (int id in _hitBreakPoints.Keys)
            {
                _context.CoroutineManager.GetCoroutineHandle(id).PostListener -= DebugBlocked;
            }

            _hitBreakPointsLock.ExitWriteLock();

            _hitBreakPointsLock.Dispose();
        }
    }
}