using System;
using System.Collections.Generic;

namespace Testflow.DataInterface.Sequence
{
    public interface IFunctionData
    {
        /// <summary>
        /// 方法类型
        /// </summary>
        FunctionType Type { get; set; }

        /// <summary>
        /// 方法所在程序集
        /// </summary>
        IAssemblyDescription Assembly { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        string MethodName { get; set; }

        /// <summary>
        /// 方法所在命名空间
        /// </summary>
        string NameSpace { get; set; }

        /// <summary>
        /// 方法参数列表信息
        /// </summary>
        IArgumentCollection ParameterType { get; set; }

        /// <summary>
        /// 方法返回值信息
        /// </summary>
        IArgumentData ReturnType { get; set; }
    }
}