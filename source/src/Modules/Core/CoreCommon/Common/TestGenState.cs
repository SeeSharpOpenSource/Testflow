namespace Testflow.CoreCommon.Common
{
    /// <summary>
    /// 测试生成状态
    /// </summary>
    public enum TestGenState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 开始生成
        /// </summary>
        Start = 1,

        /// <summary>
        /// 正在执行
        /// </summary>
        Processing = 2,

        /// <summary>
        /// 结束
        /// </summary>
        Over = 3,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 4
    }
}