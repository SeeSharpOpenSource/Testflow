 using System.Collections.Generic;
using Testflow.Data.Description;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 测试工程文件接口
    /// </summary>
    public interface ITestProject : ISequenceFlowContainer
    {
        /// <summary>
        /// 当前序列组中用到的所有Type数据
        /// </summary>
        ITypeDataCollection TypeDatas { get; set; }

        /// <summary>
        /// 模型版本号
        /// </summary>
        string ModelVersion { get; set; }

        /// <summary>
        /// 测试序列组关联的所有程序集
        /// </summary>
        IAssemblyInfoCollection Assemblies { get; set; }

        /// <summary>
        /// 执行模型，顺序执行/并行执行
        /// </summary>
        ExecutionModel ExecutionModel { get; set; }

        /// <summary>
        ///  测试序列组声明周期内所有使用到的变量
        /// </summary>
        IVariableCollection Variables { get; set; }

        /// <summary>
        /// 序列组内的变量值
        /// </summary>
        IList<IVariableInitValue> VariableValues { get; set; }

        /// <summary>
        /// 测试序列组的SetUp模块
        /// </summary>
        ISequence SetUp { get; set; }

        /// <summary>
        /// 测试工程的Setup参数配置
        /// </summary>
        ISequenceParameter SetUpParameters { get; set; }

        /// <summary>
        /// 当前测试组中包含的序列组信息
        /// </summary>
        ISequenceGroupCollection SequenceGroups { get; set; }

        /// <summary>
        /// 所有测试组中的参数配置
        /// </summary>
        IList<IParameterDataCollection> SequenceGroupParameters { get; set; }

        /// <summary>
        /// 测试序列组的TearDown模块
        /// </summary>
        ISequence TearDown { get; set; }

        /// <summary>
        /// 测试工程的TearDown参数配置
        /// </summary>
        ISequenceParameter TearDownParameters { get; set; }
    }
}