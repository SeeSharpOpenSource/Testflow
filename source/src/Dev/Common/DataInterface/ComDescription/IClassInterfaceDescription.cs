using System.Collections.Generic;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 类类型描述信息
    /// </summary>
    public interface IClassInterfaceDescription
    {
        /// <summary>
        /// 类类型描述信息
        /// </summary>
        ITypeData ClassType { get; set; }

        /// <summary>
        /// 该类中包含的所有方法信息
        /// </summary>
        IList<IFuncInterfaceDescription> Functions { get; set; }
    }
}