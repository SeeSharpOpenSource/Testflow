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
        /// <returns></returns>
        IRuntimeService CreateRuntimeService(IPropertyExtendable properties);

        /// <summary>
        /// 适用配置项
        /// </summary>
        /// <param name="properties"></param>
        void ApplyConfig(IPropertyExtendable properties);

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