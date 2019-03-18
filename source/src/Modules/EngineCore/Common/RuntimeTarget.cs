namespace Testflow.EngineCore.Common
{
    public enum RuntimePlatform
    {
        /// <summary>
        /// 基于Clr运行的Host
        /// </summary>
        Clr = 0,

        /// <summary>
        /// 通用Host运行(支持跨主机、跨语言调用)
        /// </summary>
        Common = 1
    }
}