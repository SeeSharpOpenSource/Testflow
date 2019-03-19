namespace Testflow.MasterCore.Message
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 状态消息。运行容器发送，状态维护器接收
        /// </summary>
        Status = 0,

        /// <summary>
        /// 控制指令。引擎控制、调试管理器和同步模块发送，运行容器接收
        /// </summary>
        Ctrl = 1,

        /// <summary>
        /// 调试消息。运行容器发送，调试管理器接收
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 测试生成状态消息。运行容器发送、状态维护器接收
        /// </summary>
        TestGen = 3,

        /// <summary>
        /// 远端测试生成消息。运行容器生成器发送，运行容器接收
        /// </summary>
        RmtGen = 4,

        /// <summary>
        /// 资源同步请求。运行容器收发、资源同步器发收
        /// </summary>
        Sync = 5
        
    }
}