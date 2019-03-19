namespace Testflow.MasterCore.Common
{
    /// <summary>
    /// 测试生成状态
    /// </summary>
    public enum TestGenState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle,

        /// <summary>
        /// 开始生成
        /// </summary>
        Start,

        /// <summary>
        /// 正在执行
        /// </summary>
        Processing,

        /// <summary>
        /// 结束
        /// </summary>
        Over,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}