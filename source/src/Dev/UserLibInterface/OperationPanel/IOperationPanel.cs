using System;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;

namespace Testflow.ExtensionBase.OperationPanel
{
    public interface IOperationPanel : IDisposable
    {
        ISequenceFlowContainer SequenceData { get; }

        /// <summary>
        /// 当前OI支持运行的Sequence类型
        /// </summary>
        OiSequenceType SupportSequence { get; }

        /// <summary>
        /// 初始化并显示OperationPanel
        /// </summary>
        /// <param name="sequenceData">测试序列</param>
        /// <param name="extraParams">扩展参数，定义参见接口文档</param>
        void ShowPanel(ISequenceFlowContainer sequenceData, params object[] extraParams);

        /// <summary>
        /// 测试生成开始回调方法
        /// </summary>
        /// <param name="generationInfo">生成状态信息</param>
        void TestGenerationStart(ITestGenerationInfo generationInfo);

        /// <summary>
        /// 测试生成结束回调方法
        /// </summary>
        /// <param name="generationInfo">生成结果信息</param>
        void TestGenerationOver(ITestGenerationInfo generationInfo);

        /// <summary>
        /// 测试生成开始回调方法
        /// </summary>
        void TestStart(IList<ITestResultCollection> testResults);

        /// <summary>
        /// 会话开始执行回调方法
        /// </summary>
        void SessionStart(ITestResultCollection sessionResult);

        /// <summary>
        /// 序列开始执行回调方法
        /// </summary>
        void SequenceStart(ISequenceTestResult sequenceResult);

        /// <summary>
        /// 运行中获取状态回调方法
        /// </summary>
        /// <param name="runtimeInfo">运行时状态信息，可以获取当前会话的序列运行信息</param>
        void StatusReceived(IRuntimeStatusInfo runtimeInfo);

        /// <summary>
        /// 序列执行结束回调方法
        /// </summary>
        /// <param name="sequenceResult">序列执行的结果</param>
        void SequenceOver(ISequenceTestResult sequenceResult);

        /// <summary>
        /// 会话结束回调方法
        /// </summary>
        /// <param name="sessionResult">会话执行的结果</param>
        void SessionOver(ITestResultCollection sessionResult);

        /// <summary>
        /// 测试执行结束回调方法
        /// </summary>
        /// <param name="testResults">所有会话执行的结果列表</param>
        void TestOver(IList<ITestResultCollection> testResults);
    }
}