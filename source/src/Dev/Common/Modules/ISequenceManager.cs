using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;

namespace Testflow.Modules
{
    /// <summary>
    /// 序列持久化模块
    /// </summary>
    public interface ISequenceManager : IController
    {
        /// <summary>
        /// 序列的版本号
        /// </summary>
        string Version { get; set; }

        #region 创建序列元素

        /// <summary>
        /// 创建空白的测试工程
        /// </summary>
        /// <returns>返回创建的测试工程</returns>
        ITestProject CreateTestProject();

        /// <summary>
        /// 创建空白的序列组
        /// </summary>
        /// <returns>返回创建的空白测试组</returns>
        ISequenceGroup CreateSequenceGroup();

        /// <summary>
        /// 创建空白的序列
        /// </summary>
        /// <returns></returns>
        ISequence CreateSequence();

        /// <summary>
        /// 创建空白的序列Step
        /// </summary>
        /// <returns></returns>
        ISequenceStep CreateSequenceStep();

        /// <summary>
        /// 创建空白的Argument
        /// </summary>
        /// <returns></returns>
        IArgument CreateArugment();

        /// <summary>
        /// 创建空白的FunctionData
        /// </summary>
        /// <returns></returns>
        IFunctionData CreateFunctionData(IFuncInterfaceDescription funcInterface);

        /// <summary>
        /// 创建空白的LoopCounter
        /// </summary>
        /// <returns></returns>
        ILoopCounter CreateLoopCounter();

        /// <summary>
        /// 创建空白的RetryCounter
        /// </summary>
        /// <returns></returns>
        IRetryCounter CreateRetryCounter();

        /// <summary>
        /// 创建空白的SequenceGroupParameter
        /// </summary>
        /// <returns></returns>
        ISequenceGroupParameter CreateSequenceGroupParameter();

        /// <summary>
        /// 创建空白的SequenceParameter
        /// </summary>
        /// <returns></returns>
        ISequenceParameter CreateSequenceParameter();

        /// <summary>
        /// 创建空白的SequenceStepParameter
        /// </summary>
        /// <returns></returns>
        ISequenceStepParameter CreateSequenceStepParameter();

        /// <summary>
        /// 创建空白的ParameterData
        /// </summary>
        /// <param name="argument">该ParameterData对应的Argument对象</param>
        /// <returns></returns>
        IParameterData CreateParameterData(IArgument argument);

        /// <summary>
        /// 创建空白的TypeData
        /// </summary>
        /// <returns></returns>
        ITypeData CreateTypeData();

        /// <summary>
        /// 创建空白的Variable
        /// </summary>
        /// <returns></returns>
        IVariable CreateVarialbe();

        /// <summary>
        /// 创建空白的AssemblyInfo
        /// </summary>
        /// <returns></returns>
        IAssemblyInfo CreateAssemblyInfo();

        #endregion


        #region 序列化相关

        /// <summary>
        /// 序列化测试工程
        /// </summary>
        /// <param name="project">待序列化的工程</param>
        /// <param name="target">序列化的目标</param>
        /// <param name="param">额外参数</param>
        void Serialize(ITestProject project, SerializationTarget target, params string[] param);

        /// <summary>
        /// 序列化测试序列组
        /// </summary>
        /// <param name="sequenceGroup">待序列化的测试序列组</param>
        /// <param name="target">序列化的目标</param>
        /// <param name="param">额外参数</param>
        void Serialize(ISequenceGroup sequenceGroup, SerializationTarget target, params string[] param);
        
        /// <summary>
        /// 加载测试工程
        /// </summary>
        /// <param name="source">反序列化的源</param>
        /// <param name="param">额外参数</param>
        ITestProject LoadTestProject(SerializationTarget source, params string[] param);

        /// <summary>
        /// 加载测试序列组
        /// </summary>
        /// <param name="source">反序列化的源</param>
        /// <param name="param">额外参数，如果是文件需要传入文件路径</param>
        ISequenceGroup LoadSequenceGroup(SerializationTarget source, params string[] param);

        /// <summary>
        /// 加载参数配置文件
        /// </summary>
        /// <param name="sequenceGroup">加载参数配置的目标序列组</param>
        /// <param name="forceLoad">是否强制加载，false时如果hash比对不通过会报错</param>
        /// <param name="param">额外参数，如果是文件需要传入文件路径</param>
        void LoadParameter(ISequenceGroup sequenceGroup, bool forceLoad, params string[] param);

        /// <summary>
        /// 运行时序列化
        /// </summary>
        /// <param name="testProject">待序列化的TestProject</param>
        string RuntimeSerialize(ITestProject testProject);

        /// <summary>
        /// 运行时序列化
        /// </summary>
        /// <param name="sequenceGroup">待序列化的SequenceGroup</param>
        string RuntimeSerialize(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 运行时序列化
        /// </summary>
        /// <param name="parameter">待序列化的Parameter</param>
        string RuntimeSerialize(ISequenceGroupParameter parameter);

        /// <summary>
        /// 运行时反序列化
        /// </summary>
        /// <param name="testProjectStr">待反序列化的TestProject</param>
        ITestProject RuntimeDeserialize(string testProjectStr);

        /// <summary>
        /// 运行时反序列化
        /// </summary>
        /// <param name="sequenceGroupStr">待反序列化的SequecneGroup</param>
        /// <param name="parameterStr">待反序列化的Parameter</param>
        ISequenceGroup RuntimeDeserialize(string sequenceGroupStr, string parameterStr);

        #endregion

    }
}