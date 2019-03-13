using System;

namespace Testflow.SequenceManager.Common
{
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