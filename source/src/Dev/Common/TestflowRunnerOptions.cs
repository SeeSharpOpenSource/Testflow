namespace Testflow
{
    /// <summary>
    /// 运行器选项类
    /// </summary>
    public class TestflowRunnerOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkDirectory { get; set; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public RunMode Mode { get; set; }

        /// <summary>
        /// 创建默认配置的Options实例
        /// </summary>
        public TestflowRunnerOptions()
        {
            Mode = RunMode.Full;
        }

        /// <summary>
        /// 两个Opitions是否相同
        /// </summary>
        /// <param name="options">待比较的Option</param>
        /// <returns>两个Options是否相同</returns>
        public bool Equals(TestflowRunnerOptions options)
        {
            return this.WorkDirectory.Equals(options.WorkDirectory) && this.Mode == options.Mode;
        }
    }
}