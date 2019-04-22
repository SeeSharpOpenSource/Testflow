using System;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    /// <summary>
    /// slave端流程控制元素
    /// </summary>
    internal abstract class SlaveFlowElement : IDisposable
    {
        protected readonly SlaveContext Context;

        public SlaveFlowElement(SlaveContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// 完成当前流程的任务
        /// </summary>
        public abstract void DoFlowTask();

        public SlaveFlowElement Next { get; protected set; }

        public abstract void Dispose();
    }
}