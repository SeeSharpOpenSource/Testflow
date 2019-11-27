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

        private Dictionary<string, StepTaskEntityBase> _breakPoints;

        private List<CoroutineHandle> _blockedCoroutines;

        private DebugWatchData _watchDatas;

        public DebugManager(SlaveContext context)
        {
            _context = context;
            _watchDatas = new DebugWatchData();
            _breakPoints = new Dictionary<string, StepTaskEntityBase>(Constants.DefaultRuntimeSize);
            _blockedCoroutines = new List<CoroutineHandle>(Constants.DefaultRuntimeSize);
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
            if (_blockedCoroutines.Contains(coroutineHandle))
            {
                return;
            }
            _blockedCoroutines.Add(coroutineHandle);
            coroutineHandle.PostListener += DebugBlocked;
        }

        private void Continue(int coroutineId)
        {
            CoroutineHandle coroutineHandle = _context.CoroutineManager.GetCoroutineHandle(coroutineId);
            if (!_blockedCoroutines.Contains(coroutineHandle))
            {
                return;
            }
            coroutineHandle.PostListener -= DebugBlocked;
            _blockedCoroutines.Remove(coroutineHandle);
            coroutineHandle.SetSignal();
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
            if (null != message.WatchData && null != message.WatchData.Names)
            {
                _watchDatas = message.WatchData;
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

            _context.MessageTransceiver.SendMessage(debugMessage);
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, $"Breakpoint hitted:{breakPoint}");

            stepTaskEntity.Coroutine.WaitSignal();
        }

        #endregion


        public void Dispose()
        {
            _breakPoints.Clear();
            foreach (StepTaskEntityBase stepTaskEntity in _breakPoints.Values)
            {
                stepTaskEntity.PostListener -= DebugBlocked;
            }
            _breakPoints.Clear();
            foreach (CoroutineHandle blockedCoroutine in _blockedCoroutines)
            {
                blockedCoroutine.PostListener -= DebugBlocked;
            }
            _blockedCoroutines.Clear();
        }
    }
}