using System;
using System.Collections.Generic;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 单次运行实例的数据
    /// </summary>
    public class TestInstanceData
    {
        /// <summary>
        /// 测试名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 测试描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 测试工程的名称
        /// </summary>
        public string TestProjectName { get; set; }

        /// <summary>
        /// 测试工程的描述信息
        /// </summary>
        public string TestProjectDescription { get; set; }

        /// <summary>
        /// 单词运行实例的标记值
        /// </summary>
        public string RuntimeHash { get; set; }

        /// <summary>
        /// 测试生成开始时间
        /// </summary>
        public DateTime StartGenTime { get; set; }

        /// <summary>
        /// 测试生成结束时间
        /// </summary>
        public DateTime EndGenTime { get; set; }

        /// <summary>
        /// 开始执行时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 执行结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 测试总耗时
        /// </summary>
        public double ElapsedTime { get; set; }
    }
}