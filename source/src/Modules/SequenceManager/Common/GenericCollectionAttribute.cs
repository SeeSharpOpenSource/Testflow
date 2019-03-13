using System;

namespace Testflow.SequenceManager.Common
{
    /// <summary>
    /// 标记某个集合类在运行时的真实元素类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class GenericCollectionAttribute : Attribute
    {
        public Type GenericType { get; }

        public GenericCollectionAttribute(Type genericType)
        {
            this.GenericType = genericType;
        }
    }
}