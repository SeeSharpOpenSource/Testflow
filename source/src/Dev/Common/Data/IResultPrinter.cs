using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testflow.Data
{
    //打印类接口用于给用户自定义打印类与方法
    //默认有三种输出方法：txt,xml,json
    //目前只实现.txt
    public interface IResultPrinter
    {
        /// <summary>
        /// 报表生成
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="runtimeHash">运行时哈希</param>
        void PrintReport(string filePath, string runtimeHash);
    }
}