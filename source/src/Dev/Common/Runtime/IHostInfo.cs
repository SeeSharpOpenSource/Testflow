using System;
using Testflow.Usr;

namespace Testflow.Runtime
{
    /// <summary>
    /// 主机信息
    /// </summary>
    public interface IHostInfo : IPropertyExtendable
    {
        /// <summary>
        /// 主机名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// MAC地址
        /// </summary>
        string MacAddress { get; }

        /// <summary>
        /// IP地址
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }
    }
}