namespace Testflow.CoreCommon.Common
{
    /// <summary>
    /// 测试生成状态
    /// </summary>
    public enum TestState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 开始生成
        /// </summary>
        StartGeneration = 1,

        /// <summary>
        /// 结束
        /// </summary>
        GenerationOver = 2,

        /// <summary>
        /// 测试开始
        /// </summary>
        TestStart = 3,

        /// <summary>
        /// 测试结束
        /// </summary>
        TestOver = 4,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 5
    }
}