using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 配置某个类在序列化时是否使能属性排序，只对非值类型的属性生效
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class SerializationOrderEnableAttribute : Attribute
    {
        /// <summary>
        /// 该类在序列化时是否使能属性排序
        /// </summary>
        public bool OrderEnable { get; }

        public SerializationOrderEnableAttribute(bool orderEnable)
        {
            this.OrderEnable = orderEnable;
        }

        public SerializationOrderEnableAttribute()
        {
            this.OrderEnable = true;
        }
    }
}