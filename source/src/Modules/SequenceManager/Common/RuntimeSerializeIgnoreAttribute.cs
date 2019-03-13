using System;

namespace Testflow.SequenceManager.Common
{
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