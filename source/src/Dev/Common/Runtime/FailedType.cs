namespace Testflow.Runtime
{
    /// <summary>
    /// 失败类型
    /// </summary>
    public enum FailedType
    {
        /// <summary>
        /// 测试生成失败
        /// </summary>
        TestGenFailed = -1,

        /// <summary>
        /// 断言失败
        /// </summary>
        AssertionFailed = 0,

        /// <summary>
        /// 强制失败
        /// </summary>
        ForceFailed = 1,

        /// <summary>
        /// 目标异常
        /// </summary>
        TargetError = 2,

        /// <summary>
        /// 用户取消
        /// </summary>
        Abort = 3,

        /// <summary>
        /// SetUp模块失败
        /// </summary>
        SetUpFailed = 4,

        /// <summary>
        /// 运行时异常
        /// </summary>
        RuntimeError = 5,

        /// <summary>
        /// 超时
        /// </summary>
        TimeOut = 6
    }
}