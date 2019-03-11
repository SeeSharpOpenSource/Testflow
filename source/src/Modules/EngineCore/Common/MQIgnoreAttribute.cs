using System;

namespace Testflow.EngineCore.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class MQIgnoreAttribute : Attribute
    {
        public bool Ignore { get; }

        public MQIgnoreAttribute()
        {
            this.Ignore = true;
        }

        public MQIgnoreAttribute(bool ignore)
        {
            this.Ignore = ignore;
        }
    }
}