using System;
using System.Runtime.Serialization;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.Utility.MessageUtil;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 运行时消息基类
    /// </summary>
    [Serializable]
    public abstract class MessageBase : IMessage
    {
        private static long _index = 0;

        public static void Clear()
        {
            _index = 0;
        }

        protected MessageBase(string name, int id, MessageType type)
        {
            this.Name = name;
            this.Type = type;
            this.Id = id;
            this.Time = DateTime.Now;
            SetSignature();
        }

        /// <summary>
        /// 触发的消息事件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// 消息所在的运行时Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 消息发送时刻
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 消息标签
        /// </summary>
        public long Index { get; set; }

        protected MessageBase(SerializationInfo info, StreamingContext context)
        {
            this.Name = (string) info.GetValue("Name", typeof (string));
            this.Type = (MessageType) info.GetValue("Type", typeof (MessageType));
            this.Id = (int) info.GetValue("Id", typeof (int));
            this.Time = (DateTime) info.GetValue("Time", typeof (DateTime));
            this.Index = (long) info.GetValue("Index", typeof (long));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name, typeof(string));
            info.AddValue("Type", this.Type, typeof(MessageType));
            info.AddValue("Id", this.Id, typeof(int));
            info.AddValue("Time", Time, typeof(DateTime));
            info.AddValue("Index", Index);
        }

        private void SetSignature()
        {
            const long sessionMsgCapacity = (long)1E15;
            long index = Interlocked.Increment(ref _index);
            this.Index = Id* sessionMsgCapacity + index;
        }
    }
}