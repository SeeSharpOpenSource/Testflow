using System.Runtime.Serialization;

namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 消息类
    /// </summary>
    public interface IMessage : ISerializable
    {
         /// <summary>
         /// 会话ID
         /// </summary>
         int Id { get; }
    }
}