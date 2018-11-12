using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;

namespace Testflow.DesignTime
{
    public interface IDesigntimeContext : IEntityComponent
    {
        /// <summary>
        /// 当前设计时名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 设计时导入的所有组件和程序集的映射
        /// </summary>
        IDictionary<string, IComInterfaceDescription> Components { get; }

        /// <summary>
        /// 设计时内操作的测试序列组，如果操作的是TestGroup，该值为null
        /// </summary>
        ISequenceGroup SequenceGroup { get; }

        /// <summary>
        /// 设计时内操作的TestGroup
        /// </summary>
        ITestProject TestGroup { get; set; }
    }
}