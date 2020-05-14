namespace Testflow.CoreCommon.Common
{
    public enum RuntimePlatform
    {
        /// <summary>
        /// 基于Clr内部隔离的AppDomain运行
        /// </summary>
        Clr = 0,

        /// <summary>
        /// 本地进程运行
        /// </summary>
        LocalProcess = 1,

        /// <summary>
        /// 远程Host运行
        /// </summary>
        RemoteHost = 2,
    }
}