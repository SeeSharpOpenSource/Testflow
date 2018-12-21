using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.Modules
{
    /// <summary>
    /// 引擎控制模块
    /// </summary>
    public interface IEngineController : IController
    {
        /// <summary>
        /// 创建运行时服务
        /// </summary>
        /// <param name="testProject">待运行的测试工程</param>
        IRuntimeService CreateRuntimeService(ITestProject testProject);
        
        /// <summary>
        /// 创建运行时服务
        /// </summary>
        /// <param name="sequenceGroup">待运行的测试序列组</param>
        IRuntimeService CreateRuntimeService(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 初始化某个会话中的所有调试器
        /// </summary>
        /// <param name="session">待初始化调试器的会话</param>
        void InitDebugger(IRuntimeSession session);
        
        /// <summary>
        /// 创建运行时
        /// </summary>
        IRuntimeSession CreateRuntime(ISequenceFlowContainer sequenceContainer);

        /// <summary>
        /// 终止运行时会话
        /// </summary>
        void AbortRuntime(IRuntimeSession session);

        /// <summary>
        /// 引擎开始执行所有运行时会话
        /// </summary>
        void Start();

        /// <summary>
        /// 引擎停止执行所有运行时会话
        /// </summary>
        void Stop();
    }
}