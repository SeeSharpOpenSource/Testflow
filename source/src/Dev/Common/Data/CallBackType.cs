using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testflow.Data
{
    /// <summary>
    /// 回调函数同步或者异步类型
    /// </summary>
    public enum CallBackType
    {
        /// <summary>
        /// 同步
        /// </summary>
        Synchronous = 1,

        /// <summary>
        /// 异步
        /// </summary>
        Asynchronous = 2
    }
}
