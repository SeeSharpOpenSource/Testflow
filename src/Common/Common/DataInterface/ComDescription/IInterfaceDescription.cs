using System;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 接口描述接口
    /// </summary>
    public interface IInterfaceDescription
    {
        /// <summary>
        /// 所在配置集信息
        /// </summary>
        IAssemblyDescription Assembly { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 描述信息，如果没有则为string.Empty
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 所在命名空间
        /// </summary>
        string NameSpace { get; set; }

        /// <summary>
        /// 属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 对应的Type对象
        /// </summary>
        Type Type { get; set; }
    }
}
