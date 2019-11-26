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
        private Dictionary<string, StepTaskEntityBase> _pausePoints;

        private DebugWatchData _watchDatas;

        public DebugManager(SlaveContext context)
        {
            _context = context;
            _watchDatas = new DebugWatchData();
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
                case MessageNames.StepOverName:
                    break;
                case MessageNames.StepIntoName:
                    break;
                case MessageNames.ContinueName:
                    break;
                case MessageNames.RunToEndName:
                    break;
                case MessageNames.RequestValueName:
                    break;
                case MessageNames.RefreshWatchName:
                    if (null != message.WatchData && null != message.WatchData.Names)
                    {
                        _watchDatas = message.WatchData;
                    }
                    else
                    {
                        _watchDatas.Names.Clear();
                        _watchDatas.Values.Clear();
                    }
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

        private void Pause()
        {

        }

        private void StepInto()
        {

        }

        private void StepOver()
        {

        }

        private void Continue()
        {

        }

        private void RunToEnd()
        {
        }

        #region 断点事件

        public void DebugBlocked(StepTaskEntityBase stepTaskEntity)
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
            _pausePoints.Clear();
        }
    }
}