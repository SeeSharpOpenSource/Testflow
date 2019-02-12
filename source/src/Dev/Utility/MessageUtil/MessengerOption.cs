using System;
using System.Messaging;

namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 信使选项类
    /// </summary>
    public class MessengerOption
    {
        /// <summary>
        /// 消息队列路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 主机地址
        /// </summary>
        public string HostAddress { get; set; }

        /// <summary>
        /// 信使类型
        /// </summary>
        public MessengerType Type { get; set; }

        /// <summary>
        /// 接收类型
        /// </summary>
        public ReceiveType ReceiveType { get; set; }

        /// <summary>
        /// 格式化器类型
        /// </summary>
        public FormatterType Formatter { get; set; }

        /// <summary>
        /// 目标类型
        /// </summary>
        public Type[] TargetTypes { get; set; }

        /// <summary>
        /// 创建信使选项类实例
        /// </summary>
        /// <param name="path">消息队列地址</param>
        /// <param name="targetType">目标类型</param>
        public MessengerOption(string path, params Type[] targetType)
        {
            this.Path = path;
            this.HostAddress = ".";
            this.Type = MessengerType.MSMQ;
            this.ReceiveType = ReceiveType.Asynchronous;
            this.TargetTypes = targetType;
            this.Formatter = FormatterType.Xml;
        }

        /// <summary>
        /// 判断两个Option是否相等
        /// </summary>
        /// <param name="comparer">待比较对象</param>
        public bool Equals(MessengerOption comparer)
        {
            return this.Path.Equals(comparer.Path) && this.HostAddress.Equals(comparer.HostAddress) &&
                   this.Type == comparer.Type;
        }
    }
}