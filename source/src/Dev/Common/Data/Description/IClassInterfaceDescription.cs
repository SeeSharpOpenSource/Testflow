using System.Collections.Generic;

namespace Testflow.Data.Description
{
    /// <summary>
    /// 类类型描述信息
    /// </summary>
    public interface IClassInterfaceDescription : IDescriptionData
    {
        /// <summary>
        /// 类类型描述信息的索引号
        /// </summary>
        int ClassTypeIndex { get; set; }

        /// <summary>
        /// 该类中包含的所有方法信息
        /// </summary>
        IList<IFuncInterfaceDescription> Functions { get; set; }
    }
}