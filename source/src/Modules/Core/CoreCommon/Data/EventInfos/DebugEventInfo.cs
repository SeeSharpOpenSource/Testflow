using System;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class DebugEventInfo : EventInfoBase
    {
        /// <summary>
        /// 是否为命中消息，如果为false，则为断点继续执行的操作。
        /// </summary>
        public bool IsDebugHit { get; }

        public DebugWatchData WatchData { get; }

        public CallStack BreakPoint { get; }

        public DebugEventInfo(int session, DateTime time, bool isDebugHit) : base(session, EventType.Debug, time)
        {
            this.IsDebugHit = isDebugHit;
        }

        public DebugEventInfo(DebugMessage message) : base(message.Id, EventType.Debug, message.Time)
        {
            switch (message.Name)
            {
                case MessageNames.BreakPointHitName:
                    this.BreakPoint = message.BreakPoints[0];
                    this.WatchData = message.WatchData;
                    this.IsDebugHit = true;
                    break;
                case MessageNames.StepOverName:
                case MessageNames.StepIntoName:
                case MessageNames.ContinueName:
                case MessageNames.RunToEndName:
                    this.IsDebugHit = false;
                    this.BreakPoint = null;
                    this.WatchData = null;
                    break;
                case MessageNames.RequestValueName:
                    this.IsDebugHit = false;
                    this.BreakPoint = message.BreakPoints[0];
                    this.WatchData = message.WatchData;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                    break;
            }
        }
    }
}