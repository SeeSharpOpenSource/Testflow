namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 测试序列组
    /// </summary>
    public interface ISequenceGroup : ISequenceFlowContainer
    {
        /// <summary>
        /// 测试序列组格式版本等信息
        /// </summary>
        ISequenceGroupInfo Info { get; set; }

        /// <summary>
        /// 测试序列组关联的所有程序集
        /// </summary>
        IAssemblyInfoCollection Assemblies { get; set; }

        /// <summary>
        /// 当前序列组中用到的所有Type数据
        /// </summary>
        ITypeDataCollection TypeDatas { get; set; }

        /// <summary>
        /// 测试序列组的参数
        /// </summary>
        IArgumentCollection Arguments { get; set; }

        /// <summary>
        ///  测试序列组声明周期内所有使用到的变量
        /// </summary>
        IVariableCollection Variables { get; set; }

        /// <summary>
        /// 测试序列组当前关联的测试序列组参数信息
        /// </summary>
        ISequenceGroupParameter Parameters { get; set; }

        /// <summary>
        /// 执行模型，顺序执行/并行执行
        /// </summary>
        ExecutionModel ExecutionModel { get; set; }

        /// <summary>
        /// 测试序列组的SetUp模块
        /// </summary>
        ISequence SetUp { get; set; }

        /// <summary>
        /// 测试序列组待执行的序列集合
        /// </summary>
        ISequenceCollection Sequences { get; set; }

        /// <summary>
        /// 测试序列组的TearDown模块
        /// </summary>
        ISequence TearDown { get; set; }
    }
}