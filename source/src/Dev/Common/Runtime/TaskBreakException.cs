using System;

namespace Testflow.Runtime
{
    /// <summary>
    /// 任务停止异常。在运行中途抛出该异常将停止序列的执行
    /// </summary>
    public class TaskBreakException : ApplicationException
    {
        /// <summary>
        /// 创建TaskBreakException的实例
        /// </summary>
        public TaskBreakException() : base("Task running break.")
        { }
    }
}