using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 标记某个类在序列化保存时是否被忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    internal class SerializationIgnoreAttribute : Attribute
    {
        public SerializationIgnoreAttribute()
        {
            this.Ignore = false;
        }

        public SerializationIgnoreAttribute(bool ignore)
        {
            this.Ignore = ignore;
        }

        /// <summary>
        /// 该属性序列化和反序列化时是否被忽略
        /// </summary>
        public bool Ignore { get; }
    }
}