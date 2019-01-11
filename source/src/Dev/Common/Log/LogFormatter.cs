using System;
using Testflow.Data.Sequence;

namespace Testflow.Log
{
    /// <summary>
    /// 日志格式化器
    /// </summary>
    public abstract class LogFormatter
    {
        /// <summary>
        /// 格式化异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="formatter">格式化器</param>
        /// <returns></returns>
        public abstract string Format(Exception exception, string formatter = null);

        /// <summary>
        /// 格式化变量信息
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="variableValue">变量值</param>
        /// <param name="description">变量描述</param>
        /// <param name="formatter">格式化器</param>
        /// <returns></returns>
        public abstract string Format(string variableName, string variableValue,string description = null, string formatter = null);
    }
}