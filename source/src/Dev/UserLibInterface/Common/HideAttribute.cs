using System;

namespace Testflow.ExtensionBase.Common
{
    /// <summary>
    /// 是否隐藏某个类/属性/方法
    /// </summary>
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
