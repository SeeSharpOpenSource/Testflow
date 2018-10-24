using System;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    public interface IVariable
    {
        string Name { get; set; }
        IAssemblyDescription Assembly { get; set; }
        Type Type { get; set; }
        VariableType VariableType { get; set; }
        
        /// <summary>
        /// 是否定义时初始化
        /// </summary>
        bool InitWhenDefine { get; set; }
    }
}