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
        ITestProject CreateTestProject(params object[] param);

        /// <summary>
        /// 创建空白的序列组
        /// </summary>
        /// <returns>返回创建的空白测试组</returns>
        ISequenceGroup CreateSequenceGroup(params object[] param);

        /// <summary>
        /// 创建空白的序列
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ISequence CreateSequence(params object[] param);

        /// <summary>
        /// 创建空白的序列Step
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ISequenceStep CreateSequenceStep(params object[] param);

        /// <summary>
        /// 创建空白的Argument
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IArgument CreateArugment(params object[] param);

        /// <summary>
        /// 创建空白的FunctionData
        /// </summary>
        /// <param name="funcInterface"></param>
        /// <returns></returns>
        IFunctionData CreateFunctionData(IFuncInterfaceDescription funcInterface);

        /// <summary>
        /// 创建空白的LoopCounter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ILoopCounter CreateLoopCounter(params object[] param);

        /// <summary>
        /// 创建空白的RetryCounter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IRetryCounter CreateRetryCounter(params object[] param);

        /// <summary>
        /// 创建空白的SequenceGroupParameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ISequenceGroupParameter CreateSequenceGroupParameter(params object[] param);

        /// <summary>
        /// 创建空白的SequenceParameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ISequenceParameter CreateSequenceParameter(params object[] param);

        /// <summary>
        /// 创建空白的SequenceStepParameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ISequenceStepParameter CreateSequenceStepParameter(params object[] param);

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
        /// 反序列化测试工程
        /// </summary>
        /// <param name="source">反序列化的源</param>
        /// <param name="param">额外参数</param>
        ITestProject DeserializeTestProject(SerializationTarget source, params string[] param);

        /// <summary>
        /// 反序列化测试序列组
        /// </summary>
        /// <param name="source">反序列化的源</param>
        /// <param name="param">额外参数</param>
        ISequenceGroup DeserializeSequenceGroup(SerializationTarget source, params string[] param);

        #endregion

    }
}