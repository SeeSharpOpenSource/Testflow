using System;

namespace Testflow.ExtensionBase.Common
{
    /// <summary>
    /// 用于断言声明的注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AssertionAttribute : Attribute
    {
    }
}