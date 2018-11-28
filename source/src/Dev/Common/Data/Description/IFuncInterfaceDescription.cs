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
        /// 对应的Type对象的索引号
        /// </summary>
        int ClassTypeIndex { get; set; }

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
        /// 方法所在实例，静态方法时该参数为null
        /// </summary>
        IInstanceDescription Instance { set; get; }

        /// <summary>
        /// 方法被显示的签名字符串
        /// </summary>
        string Signature { get; set; }

        /// <summary>
        /// 方法被调用时的字符串构造器
        /// </summary>
        string FunctionCallingFormat { get; set; }
    }
}
