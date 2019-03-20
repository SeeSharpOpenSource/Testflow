using System;
using Testflow.Utility.MessageUtil;

namespace Testflow.CoreCommon.Data.EventInfos
{
    /// <summary>
    /// 引擎内部的事件基类
    /// </summary>
    public abstract class EventInfoBase
    {
        public int Session { get; }

        public EventType Type { get; }

        public DateTime TimeStamp { get; }

        public EventInfoBase(int session, EventType type, DateTime timeStamp)
        {
            this.Session = session;
            this.Type = type;
            this.TimeStamp = timeStamp;
        }
    }
}