using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 标记某个属性或者类在运行时序列化时是否应该被忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    internal class RuntimeSerializeIgnoreAttribute : Attribute
    {
        public bool Ignore { get; }

        public RuntimeSerializeIgnoreAttribute(bool ignore)
        {
            this.Ignore = ignore;
        }

        public RuntimeSerializeIgnoreAttribute()
        {
            this.Ignore = true;
        }

    }
}