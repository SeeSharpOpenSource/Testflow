namespace Testflow.CoreCommon.Data.EventInfos
{
    public enum EventType
    {
        /// <summary>
        /// 内部异常
        /// </summary>
        Exception = -1,

        /// <summary>
        /// 测试生成
        /// </summary>
        TestGen = 0,

        /// <summary>
        /// 同步消息
        /// </summary>
        Sync = 1,

        /// <summary>
        /// 调试状态消息
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 用户停止
        /// </summary>
        Abort = 3
    }
}