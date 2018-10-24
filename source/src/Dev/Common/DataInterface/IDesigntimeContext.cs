using System.Collections.Generic;
using Testflow.DataInterface.ComDescription;
using Testflow.DataInterface.Sequence;

namespace Testflow.DataInterface
{
    public interface IDesigntimeContext
    {
        /// <summary>
        /// 当前设计时的会话ID
        /// </summary>
        int SessionId { get; set; }

        /// <summary>
        /// 设计时导入的所有组件和其ID的映射
        /// </summary>
        Dictionary<int, IComInterfaceDescription> Components { get; }

        /// <summary>
        /// 设计时内操作的测试序列组
        /// </summary>
        ISequenceGroupData SequenceGroup { get; }
    }
}