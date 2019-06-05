using System;

namespace Testflow.Usr.Common
{
    /// <summary>
    /// 回调方法注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallBackAttribute : Attribute
    {
        /// <summary>
        /// 回调方法名
        /// </summary>
        public CallBackAttribute(string command)
        {
            this.Command = command;
        }

        /// <summary>
        /// 回调命令
        /// </summary>
        public string Command { get; }
    }
}