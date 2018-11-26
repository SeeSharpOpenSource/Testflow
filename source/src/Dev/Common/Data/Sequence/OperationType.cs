namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 运行行为，正常运行/跳过/强制成功/强制失败
    /// </summary>
    public enum RunBehavior
    {
        /// <summary>
        /// 正常运行
        /// </summary>
        Run,

        /// <summary>
        /// 跳过该步骤
        /// </summary>
        Skip,

        /// <summary>
        /// 强制成功
        /// </summary>
        ForceSuccess,

        /// <summary>
        /// 强制失败
        /// </summary>
        ForceFailed
    }
}