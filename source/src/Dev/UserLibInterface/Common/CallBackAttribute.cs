using System;
using Testflow.Data;

namespace Testflow.Usr.Common
{
    /// <summary>
    /// 回调类型注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallBackAttribute : Attribute
    {
        /// <summary>
        /// 回调类型
        /// </summary>
        public CallBackAttribute(CallBackType callBackType)
        {
            this.CallBackType = callBackType;
        }

        /// <summary>
        /// 回调类型，同步还是异步
        /// </summary>
        public CallBackType CallBackType { get; }
    }
}