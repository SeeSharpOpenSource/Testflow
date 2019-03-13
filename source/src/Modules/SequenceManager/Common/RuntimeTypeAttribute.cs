using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 标记某个属性在运行时的真实类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class RuntimeTypeAttribute : Attribute
    {
        public Type RealType { get; }

        public RuntimeTypeAttribute(Type realType)
        {
            this.RealType = realType;
        }
    }
}