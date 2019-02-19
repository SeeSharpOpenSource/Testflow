using System.Collections.Generic;

namespace Testflow.Data.Description
{
    /// <summary>
    /// 方法描述接口
    /// </summary>
    public interface IFuncInterfaceDescription : IDescriptionData
    {
        /// <summary>
        /// 属于那种类型
        /// </summary>
        FunctionType ArgumentType { get; set; }

        /// <summary>
        /// 组件的索引在全局的索引号
        /// </summary>
        int ComponentIndex { get; set; }

        /// <summary>
        /// 对应的Type对象
        /// </summary>
        ITypeData ClassType { get; set; }

        /// <summary>
        /// 是否是泛型方法
        /// </summary>
        bool IsGeneric { get; set; }

        /// <summary>
        /// 方法返回值信息
        /// </summary>
        IArgumentDescription Return { get; set; }

        /// <summary>
        /// 入参列表信息
        /// </summary>
        IList<IArgumentDescription> Arguments { get; set; }

        /// <summary>
        /// 方法被显示的签名字符串
        /// </summary>
        string Signature { get; set; }
    }
}
