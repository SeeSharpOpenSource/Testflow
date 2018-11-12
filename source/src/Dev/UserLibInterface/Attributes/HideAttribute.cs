using System;

namespace Testflow.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class HideAttribute : Attribute
    {
        public bool Hide { get; }
        public HideAttribute()
        {
            this.Hide = false;
        }

        public HideAttribute(bool isHide)
        {
            this.Hide = isHide;
        }
    }
}
