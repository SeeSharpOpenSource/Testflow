using System;

namespace Testflow.Usr.Common
{
    /// <summary>
    /// 在Testflow中被使用的元素注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |　AttributeTargets.Method | AttributeTargets.Property)]
    public class TestflowElementAttribute : Attribute
    {
        /// <summary>
        /// 元素是否为Testflow的可用项
        /// </summary>
        public bool IsTestflowElement { get; }

        /// <summary>
        /// 标记某个类/方法/属性会作为Testflow的使用项目
        /// </summary>
        public TestflowElementAttribute()
        {
            this.IsTestflowElement = true;
        }

        /// <summary>
        /// 是否标记某个类/方法/属性会作为Testflow的使用项目
        /// </summary>
        /// <param name="isTestflowElement">是否标记目标为Testflow的可用项目</param>
        public TestflowElementAttribute(bool isTestflowElement)
        {
            this.IsTestflowElement = isTestflowElement;
        }
    }
}