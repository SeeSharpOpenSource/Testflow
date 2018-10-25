using System;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 变量数据
    /// </summary>
    public interface IVariableData
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 变量所在的程序集
        /// </summary>
        IAssemblyDescription Assembly { get; set; }

        /// <summary>
        /// 变量的Type对象
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// 变量的类型
        /// </summary>
        VariableType VariableType { get; set; }
        
        /// <summary>
        /// 变量的值，如果没有则为null
        /// </summary>
        object Value { get; set; }
    }
}