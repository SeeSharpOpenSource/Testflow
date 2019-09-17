using System;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 失败信息类
    /// </summary>
    public interface IFailedInfo
    {
        /// <summary>
        /// 失败类型
        /// </summary>
        FailedType Type { get; set; }

        /// <summary>
        /// 错误描述信息
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 错误源，FailedType为UnHandledException和RuntimeError有效
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// 错误堆栈，FailedType为UnHandledException和RuntimeError有效
        /// </summary>
        string StackTrace { get; set; }

        /// <summary>
        /// 异常类型，FailedType为UnHandledException和RuntimeError有效
        /// </summary>
        string ExceptionType { get; set; }
    }
}
