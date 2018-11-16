using System;

namespace Testflow.Attributes
{
    /// <summary>
    /// Testflow中会被使用的数据类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TestflowTypeAttribute : Attribute
    {
        /// <summary>
        /// 该类是否是Testflow中会被使用的数据类型
        /// </summary>
        public bool IsTestflowDataType { get; }

        /// <summary>
        /// 标记该类为Testflow中会被使用的数据类型
        /// </summary>
        public TestflowTypeAttribute()
        {
            this.IsTestflowDataType = true;
        }

        /// <summary>
        /// 是否标记该类为Testflow中会被使用的数据类型
        /// </summary>
        /// <param name="isDataType">该类是否为Testflow中会被使用的数据类型</param>
        public TestflowTypeAttribute(bool isDataType)
        {
            this.IsTestflowDataType = isDataType;
        }
    }
}