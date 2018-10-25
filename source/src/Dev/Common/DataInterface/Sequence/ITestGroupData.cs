using System.Collections.Generic;

namespace Testflow.DataInterface.Sequence
{
    public interface ITestGroupData
    {
        /// <summary>
        /// 测试序列组关联的所有程序集
        /// </summary>
        IList<IAssemblyDescription> Assemblies { get; set; }

        /// <summary>
        /// 执行模型，顺序执行/并行执行
        /// </summary>
        ExecutionModel ExecutionModel { get; set; }

        /// <summary>
        ///  测试序列组声明周期内所有使用到的变量
        /// </summary>
        IVariableCollection Variables { get; set; }

        /// <summary>
        /// 测试序列组的SetUp模块
        /// </summary>
        ISequenceData SetUp { get; set; }

        /// <summary>
        /// 当前测试组中包含的序列组信息
        /// </summary>
        IList<ISequenceGroupData> SequenceGroups { get; set; }

        /// <summary>
        /// 所有测试组中的参数配置
        /// </summary>
        IList<IParameterDataCollection> SequenceParameters { get; set; }

            /// <summary>
        /// 测试序列组的TearDown模块
        /// </summary>
        ISequenceData TearDown { get; set; }
    }
}