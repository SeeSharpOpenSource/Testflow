using System.Collections.Generic;
using Testflow.DataInterface.ComDescription;
using Testflow.DataInterface.Sequence;

namespace Testflow.DesignTime
{
    public interface IDesigntimeContext
    {
        string Name { get; }

        /// <summary>
        /// 当前设计时的会话ID
        /// </summary>
        int SessionId { get; set; }

        /// <summary>
        /// 设计时导入的所有组件和程序集的映射
        /// </summary>
        Dictionary<string, IComInterfaceDescription> Components { get; }

        /// <summary>
        /// 设计时内操作的测试序列组，如果操作的是TestGroup，该值为null
        /// </summary>
        ISequenceGroupData SequenceGroup { get; }

        /// <summary>
        /// 设计时内操作的TestGroup
        /// </summary>
        ITestGroupData TestGroup { get; set; }

        IDesigntimeContext GetContext(string contextName);
    }
}