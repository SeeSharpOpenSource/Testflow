using System;

namespace Testflow.ExtensionBase.Common
{
    /// <summary>
    /// 描述信息
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }
        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}