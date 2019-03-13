using System;

namespace Testflow.SequenceManager.Common
{
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