using Testflow.Data;
using Testflow.Data.Sequence;

namespace Testflow.Modules
{
    /// <summary>
    /// 结果管理模块
    /// </summary>
    public interface IResultManager : IController
    {
        /// <summary>
        /// 根据用户所指定文件类型输出报告
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="runtimeHash">运行时哈希值</param>
        /// <param name="reportType">报告类型枚举，有txt文件，xml文件，json文件，和自定义报告</param>
        /// <param name="sequenceData">待生成报告的序列对象</param>
        /// <param name="customPrinter">可选参数，用于当reportType为custom的时候</param>
        void PrintReport(string filePath, string runtimeHash, ReportType reportType, ISequenceFlowContainer sequenceData = null, IResultPrinter customPrinter = null);
    }
}