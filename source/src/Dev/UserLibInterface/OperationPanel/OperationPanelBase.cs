using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ExtensionBase.OperationPanel
{
    public abstract class OperationPanelBase : IDisposable
    {
        public ISequenceFlowContainer SequenceData { get; private set; }

        public RunnerType RunnerType { get; private set; }

        protected OperationPanelBase()
        {
            I18NOption i18NOption = new I18NOption(Assembly.GetAssembly(this.GetType()), "i18n_userlib_zh",
                "i18n_userlib_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
        }

        /// <summary>
        /// 初始化并显示OperationPanel
        /// </summary>
        /// <param name="sequenceData">测试序列</param>
        /// <param name="extraParams">扩展参数，定义参见接口文档</param>
        public virtual void ShowPanel(ISequenceFlowContainer sequenceData, params object[] extraParams)
        {
            if (sequenceData is ITestProject)
            {
                RunnerType = RunnerType.TestProject;
            }
            else if (sequenceData is ISequenceGroup)
            {
                RunnerType = RunnerType.SequenceGroup;
            }
            else
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(-1, i18N.GetStr("InvalidSequence"));
            }
            this.SequenceData = sequenceData;
        }

        /// <summary>
        /// 测试生成开始回调方法
        /// </summary>
        /// <param name="generationInfo">生成状态信息</param>
        public abstract void TestGenerationStart(ITestGenerationInfo generationInfo);

        /// <summary>
        /// 测试生成结束回调方法
        /// </summary>
        /// <param name="generationInfo">生成结果信息</param>
        public abstract void TestGenerationOver(ITestGenerationInfo generationInfo);
        
        /// <summary>
        /// 测试生成失败回调方法
        /// </summary>
        /// <param name="generationInfo">测试生成状态信息</param>
        public abstract void TestGenerationFailed(ITestGenerationInfo generationInfo);

        /// <summary>
        /// 测试生成开始回调方法
        /// </summary>
        /// <param name="startTime">测试开始执行时间</param>
        public abstract void TestStart(DateTime startTime);

        /// <summary>
        /// 会话开始执行回调方法
        /// </summary>
        /// <param name="session">会话的ID号</param>
        /// <param name="time">会话开始的时间</param>
        public abstract void SessionStart(int session, DateTime time);

        /// <summary>
        /// 序列开始执行回调方法
        /// </summary>
        /// <param name="sequence">开始执行的序列</param>
        /// <param name="time">开始执行的时间</param>
        public abstract void SequenceStart(ISequence sequence, DateTime time);

        /// <summary>
        /// 运行中获取状态回调方法
        /// </summary>
        /// <param name="currentStep">当前执行的Step</param>
        /// <param name="runtimeInfo">运行时状态信息，可以获取当前会话的序列运行信息</param>
        public abstract void StatusReceived(ISequenceStep currentStep, IRuntimeStatusInfo runtimeInfo);

        /// <summary>
        /// 序列执行结束回调方法
        /// </summary>
        /// <param name="sequenceResult">序列执行的结果</param>
        public abstract void SequenceOver(ISequenceTestResult sequenceResult);

        /// <summary>
        /// 会话结束回调方法
        /// </summary>
        /// <param name="sessionResult">会话执行的结果</param>
        public abstract void SessionOver(ITestResultCollection sessionResult);

        /// <summary>
        /// 测试执行结束回调方法
        /// </summary>
        /// <param name="testResults">所有会话执行的结果列表</param>
        public abstract void TestOver(IList<ITestResultCollection> testResults);

        /// <summary>
        /// 发生异常时回调方法
        /// </summary>
        /// <param name="errorStep">出现错误的步骤</param>
        /// <param name="failedInfo">错误的详细信息</param>
        public abstract void ErrorOccured(ISequenceStep errorStep, IFailedInfo failedInfo);

        /// <summary>
        /// 释放OperationPanel资源
        /// </summary>
        public abstract void Dispose();
    }
}