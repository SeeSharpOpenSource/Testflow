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
        Normal = 0,

        /// <summary>
        /// 跳过该步骤
        /// </summary>
        Skip = 1,

        /// <summary>
        /// 强制成功
        /// </summary>
        ForceSuccess = 2,

        /// <summary>
        /// 强制失败
        /// </summary>
        ForceFailed = 3
    }
}