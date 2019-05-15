using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.DesignTime;

namespace Testflow.Runtime
{
    /// <summary>
    /// 单个测试序列的调试会话
    /// </summary>
    public interface IDebuggerHandle
    {

        #region 调试流程控制

        /// <summary>
        /// 如果存在子Step，则进入子Step序列，否则执行到下个同级Step
        /// </summary>
        void StepInto();

        /// <summary>
        /// 执行到下个同级Step
        /// </summary>
        void StepOver();

        /// <summary>
        /// 继续执行，直到下一个断点
        /// </summary>
        void Continue();

        /// <summary>
        /// 跳过当前序列的所有断点
        /// </summary>
        void SkipAllBreakPoints();

        #endregion


    }
}