using System;

namespace Testflow.Usr.Common
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