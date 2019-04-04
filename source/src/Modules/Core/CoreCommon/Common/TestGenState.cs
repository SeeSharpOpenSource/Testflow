namespace Testflow.CoreCommon.Common
{
    /// <summary>
    /// 测试生成状态
    /// </summary>
    public enum TestGenState
    {
        /// <summary>
        /// 开始生成
        /// </summary>
        StartGeneration = 0,

        /// <summary>
        /// 结束
        /// </summary>
        GenerationOver = 1,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 2
    }
}