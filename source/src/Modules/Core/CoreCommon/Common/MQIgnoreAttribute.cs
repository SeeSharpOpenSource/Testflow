using System;

namespace Testflow.CoreCommon.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MQIgnoreAttribute : Attribute
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