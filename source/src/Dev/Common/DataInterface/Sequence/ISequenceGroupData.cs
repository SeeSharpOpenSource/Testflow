using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    public interface ISequenceGroupData : ICloneable
    {
        /// <summary>
        /// 测试序列组的名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 测试序列组的描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 测试序列组格式版本等信息
        /// </summary>
        ISequenceGroupInfo Info { get; set; }

        /// <summary>
        /// 测试序列组关联的所有程序集
        /// </summary>
        IList<IAssemblyDescription> Assemblies { get; set; }

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
        ISequenceData SetUp { get; set; }

        /// <summary>
        /// 测试序列组待执行的序列集合
        /// </summary>
        ISequenceCollection Sequences { get; set; }

        /// <summary>
        /// 测试序列组的TearDown模块
        /// </summary>
        ISequenceData TearDown { get; set; }
        
        /// <summary>
        /// 初始化一个空白的测试序列组
        /// </summary>
        void Initialize();

        /// <summary>
        /// 对测试序列组执行变更后更新所有签名信息
        /// </summary>
        void RefreshSignature();
    }
}