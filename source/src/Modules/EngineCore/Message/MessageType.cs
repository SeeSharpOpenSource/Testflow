namespace Testflow.EngineCore.Message
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 状态消息
        /// </summary>
        Status = 0,

        /// <summary>
        /// 控制指令
        /// </summary>
        Ctrl = 1,

        /// <summary>
        /// 调试消息
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 测试生成状态消息
        /// </summary>
        TestGen = 3,

        /// <summary>
        /// 远端测试生成消息
        /// </summary>
        RmtGen = 4
    }
}