using System.Collections.Generic;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 组件接口描述类，保存某个组件接口的描述信息。该类由ComInterfaceLoader生成。
    /// </summary>
    public interface IComInterfaceDescription
    {
        /// <summary>
        /// 组件在当前会话中的ID
        /// </summary>
        int ComponentId { get; set; }

        /// <summary>
        /// 组件在Testflow中的签名
        /// </summary>
        string Signature { get; set; }

        /// <summary>
        /// 配置及信息
        /// </summary>
        IAssemblyDescription Assembly { get; set; }

        /// <summary>
        /// 当前程序集中包含的所有方法列表
        /// </summary>
        IList<IFuncInterfaceDescription> Functions { get; set; }

    }
}
