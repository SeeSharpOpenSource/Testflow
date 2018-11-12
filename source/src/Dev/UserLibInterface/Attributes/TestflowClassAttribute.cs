using System;

namespace Testflow.Attributes
{
    /// <summary>
    /// Testflow允许的类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TestflowClassAttribute : Attribute
    {
        public bool IsTestflowDataType { get; }
        public TestflowClassAttribute()
        {
            this.IsTestflowDataType = true;
        }

        public TestflowClassAttribute(bool isDataType)
        {
            this.IsTestflowDataType = isDataType;
        }
    }
}