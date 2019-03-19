using System;
using System.Runtime.Serialization;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Utility.MessageUtil;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 运行时消息基类
    /// </summary>
    public abstract class MessageBase : IMessage, ISerializable
    {
        protected MessageBase(string name, MessageType type)
        {
            this.Name = name;
            this.Type = type;
            this.Id = CommonConst.TestGroupSession;
            this.Time = DateTime.Now;
        }

        protected MessageBase(string name, int id, MessageType type)
        {
            this.Name = name;
            this.Type = type;
            this.Id = id;
            this.Time = DateTime.Now;
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

        protected MessageBase(SerializationInfo info, StreamingContext context)
        {
            this.Name = (string) info.GetValue("Name", typeof (string));
            this.Type = (MessageType) info.GetValue("Type", typeof (MessageType));
            this.Id = (int) info.GetValue("Id", typeof (int));
            this.Time = (DateTime) info.GetValue("Time", typeof (DateTime));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name, typeof(string));
            info.AddValue("Type", this.Type, typeof(MessageType));
            info.AddValue("Id", this.Id, typeof(int));
            info.AddValue("Time", Time, typeof(DateTime));
        }
    }
}