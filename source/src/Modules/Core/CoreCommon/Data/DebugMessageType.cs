namespace Testflow.CoreCommon.Data
{
    public enum DebugMessageType
    {
        /// <summary>
        /// 添加断点
        /// </summary>
        AddBreakPoint = 0,

        /// <summary>
        /// 删除断点
        /// </summary>
        RemoveBreakPoint = 1,

        /// <summary>
        /// 更新Watch变量
        /// </summary>
        RefreshWatch = 2,

        /// <summary>
        /// 断点命中
        /// </summary>
        BreakPointHitted = 3
    }
}