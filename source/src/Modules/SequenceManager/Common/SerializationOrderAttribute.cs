using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 配置在序列化过程中某个属性的顺序，如果序列化的属性未配置该字段，则这些属性排在最后
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class SerializationOrderAttribute : Attribute
    {
        /// <summary>
        /// 配置序列化时该元素的位置，从0开始，数字越小越靠前
        /// </summary>
        public int Order { get; }
        public SerializationOrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}