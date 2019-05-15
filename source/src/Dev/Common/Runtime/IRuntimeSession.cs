using System;
using Testflow.Usr;
using Testflow.Data;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 运行时单个测试用例组运行的会话
    /// </summary>
    public interface IRuntimeSession : IEntityComponent
    {
        /// <summary>
        /// 当前运行时的上下文信息
        /// </summary>
        IRuntimeContext Context { get; }

        /// <summary>
        /// 运行时状态
        /// </summary>
        RuntimeState State { get; }

        /// <summary>
        /// 当前会话的ID，该ID和运行的序列组的ID一致
        /// </summary>
        int ID { get; }

        #region Status相关事件

        /// <summary>
        /// 测试生成开始事件
        /// </summary>
        event RuntimeDelegate.SessionGenerationAction SessionGenerationStart;

        /// <summary>
        /// 测试生成中间事件，生成过程中会不间断生成该事件
        /// </summary>
        event RuntimeDelegate.SessionGenerationAction SessionGenerationReport;

        /// <summary>
        /// 测试生成结束事件
        /// </summary>
        event RuntimeDelegate.SessionGenerationAction SessionGenerationEnd;

        /// <summary>
        /// 整个测试工程的测试开始时触发
        /// </summary>
        event RuntimeDelegate.SessionStatusAction SessionStart;

        /// <summary>
        /// Events raised when a sequence is start and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.SequenceStatusAction SequenceStarted;

        /// <summary>
        /// Events raised when receive runtime status information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.StatusReceivedAction StatusReceived;

        /// <summary>
        /// Events raised when a sequence is over and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.SequenceStatusAction SequenceOver;
//
//        /// <summary>
//        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
//        /// </summary>
//        event RuntimeDelegate.StatusReceivedAction SequenceFailed;
        
        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.SessionStatusAction SessionOver;

        /// <summary>
        /// 断点命中事件，当某个断点被命中时触发
        /// </summary>
        event RuntimeDelegate.BreakPointHittedAction BreakPointHitted;

        #endregion

        #region 运行时控制

        /// <summary>
        /// 开始运行
        /// </summary>
        void Start();

        /// <summary>
        /// 强制停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 根据调用堆栈获取当前执行的Step
        /// </summary>
        /// <param name="stack">当前的执行堆栈</param>
        /// <returns></returns>
        ISequenceStep GetSequenceStep(ICallStack stack);

        #endregion

        #region 状态控制

        /// <summary>
        /// 强制更新所有的状态标识
        /// </summary>
        void ForceRefreshStatus();

        #endregion

        #region 调试控制

        /// <summary>
        /// 在某个序列的某个Step上添加断点
        /// </summary>
        /// <param name="sequence">待调试的序列</param>
        /// <param name="sequenceStep">断点位置</param>
        bool AddBreakPoint(ISequence sequence, ISequenceStep sequenceStep);

        /// <summary>
        /// 删除某个序列的某个Step上的断点
        /// </summary>
        /// <param name="sequence">调试的序列</param>
        /// <param name="sequenceStep">断点位置</param>
        bool RemoveBreakPoint(ISequence sequence, ISequenceStep sequenceStep);

        /// <summary>
        /// 删除某个序列上的所有断点
        /// </summary>
        /// <param name="sequence"></param>
        void RemoveAllBreakPoint(ISequence sequence);

        /// <summary>
        /// 返回某个Step是否包含调试断点
        /// </summary>
        /// <param name="sequence">调试的测试序列</param>
        /// <param name="sequenceStep">判断是否存在断点的Step</param>
        /// <returns></returns>
        bool HasBreakPoint(ISequence sequence, ISequenceStep sequenceStep);

        /// <summary>
        /// 根据索引号获取对应的Sequence
        /// </summary>
        /// <param name="index">序列对应的索引号</param>
        /// <returns>Sequence对象</returns>
        ISequence GetSequence(int index);

        #endregion

    }
}