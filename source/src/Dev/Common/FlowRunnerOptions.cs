namespace Testflow
{
    /// <summary>
    /// 运行器选项类
    /// </summary>
    public class FlowRunnerOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkDirectory { get; set; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public RunMode Mode { get; set; }
        
        public FlowRunnerOptions()
        {
            Mode = RunMode.Full;
        } 
    }
}