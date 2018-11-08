namespace Testflow.Runtime
{
    public interface IRunningContainer
    {
        /// <summary>
        /// 当前运行会话的线程ID
        /// </summary>
        int ThreadID { get; }

        void Start();

        void Stop();

        /// <summary>
        /// 调试器
        /// </summary>
        IStepDebugger Debugger { get; }

        /// <summary>
        /// 运行时类型，运行/调试
        /// </summary>
        RunTimeType Type { get; set; }
    }
}