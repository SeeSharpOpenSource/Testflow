namespace Testflow.Runtime
{
    /// <summary>
    /// 失败类型
    /// </summary>
    public enum FailedType
    {
        /// <summary>
        /// 断言失败
        /// </summary>
        AssertionFailed = 0,

        /// <summary>
        /// 未捕获的异常
        /// </summary>
        UnHandledException = 1,

        /// <summary>
        /// 用户取消
        /// </summary>
        Abort = 2,

        /// <summary>
        /// SetUp模块失败
        /// </summary>
        SetUpFailed = 3,

        /// <summary>
        /// 运行时异常
        /// </summary>
        RuntimeFailed = 4,

        /// <summary>
        /// 超时
        /// </summary>
        TimeOut = 5
    }
}