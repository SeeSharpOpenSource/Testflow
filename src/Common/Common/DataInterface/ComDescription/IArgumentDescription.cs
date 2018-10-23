using System;

namespace Testflow.DataInterface.ComDescription
{
    public interface IArgumentDescription
    {
        /// <summary>
        /// 方法所在配置集信息
        /// </summary>
        IAssemblyDescription Assembly { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 参数属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 参数的Type对象
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// 参数默认值
        /// </summary>
        object DefaultValue { get; set; }
    }
}