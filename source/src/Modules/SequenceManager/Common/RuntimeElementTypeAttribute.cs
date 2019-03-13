using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 标记某个集合类运行时的元素类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class RuntimeElementTypeAttribute : Attribute
    {
        public Type RealType { get; }

        public RuntimeElementTypeAttribute(Type realType)
        {
            this.RealType = realType;
        }
    }
}