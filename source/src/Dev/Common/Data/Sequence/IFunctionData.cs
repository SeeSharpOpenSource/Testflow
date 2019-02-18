using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Testflow.Common;
using Testflow.Data.Description;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 保存一个步骤中调用的方法信息
    /// </summary>
    public interface IFunctionData : ICloneableClass<IFunctionData>
    {
        /// <summary>
        /// 方法类型
        /// </summary>
        FunctionType Type { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        string MethodName { get; set; }

        /// <summary>
        /// 方法所在类
        /// </summary>
        ITypeData ClassType { get; set; }

        /// <summary>
        /// 方法参数列表信息
        /// </summary>
        IArgumentCollection ParameterType { get; set; }

        /// <summary>
        /// 该Function的参数配置信息
        /// </summary>
        [XmlIgnore]
        IParameterDataCollection Parameters { get; set; }

        /// <summary>
        /// 方法所在类的实例，静态方法时为空
        /// </summary>
        string Instance { get; set; }

        /// <summary>
        /// 保存返回值的变量名
        /// </summary>
        string Return { get; set; }

        /// <summary>
        /// 方法返回值信息
        /// </summary>
        IArgument ReturnType { get; set; }

        /// <summary>
        /// 关联的Description信息
        /// </summary>
        [XmlIgnore]
        IFuncInterfaceDescription Description { get; set; }
    }
}