using Testflow.Usr.Common;

namespace Testflow.FunctionDefinitions
{
    [Hide]
    public static class StepFunctions
    {
        /// <summary>
        /// 调用其他序列
        /// </summary>
        /// <param name="sequenceLocation">序列所在位置</param>
        public static void SequenceCall(string sequenceLocation){}

        /// <summary>
        /// 调用其他序列
        /// </summary>
        /// <param name="callStackStr">goto的step的堆栈信息</param>
        /// <param name="stepName">Goto的Step名称</param>
        public static void Goto(string callStackStr, string stepName) { }

        /// <summary>
        /// 多线程运行
        /// </summary>
        /// <param name="threadCount">线程的个数</param>
        /// <param name="threadIdVariable">保存线程索引的变量</param>
        public static void MultiThreadTask(int threadCount, string threadIdVariable){}

        /// <summary>
        /// 批处理运行
        /// </summary>
        /// <param name="threadCount">线程的个数</param>
        /// <param name="threadVariable">保存线程索引的变量</param>
        public static void BatchTask(int threadCount, string threadVariable) { }

        /// <summary>
        /// 计时器运行任务
        /// </summary>
        /// <param name="maxCount">运行次数</param>
        /// <param name="timerVariable">当前运行索引号的变量</param>
        /// <param name="startDelay">开始延迟时间</param>
        /// <param name="interval">中间延迟时间</param>
        public static void TimerTask(int maxCount, string timerVariable, int startDelay, int interval) {}
    }
}