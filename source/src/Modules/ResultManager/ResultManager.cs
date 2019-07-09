using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Modules;
using Testflow.Usr;
using System.IO;
using Testflow.ResultManager.Common;

namespace Testflow.ResultManager
{
    public class ResultManager : IResultManager
    {
        private IDataMaintainer _dataMaintainer;
        #region 初始化

        public IModuleConfigData ConfigData { get; set; }
        
        public void ApplyConfig(IModuleConfigData configData)
        {
            // TODO
        }

        public void DesigntimeInitialize()
        {
            if(_dataMaintainer == null)
            {
                _dataMaintainer = TestflowRunner.GetInstance().DataMaintainer;
            }       
        }

        public void RuntimeInitialize()
        {
            if (_dataMaintainer == null)
            {
                _dataMaintainer = TestflowRunner.GetInstance().DataMaintainer;
            }
        }

        #endregion
        
        /// <summary>
        /// 根据用户所指定文件类型输出报告
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="runtimeHash">运行时哈希值</param>
        /// <param name="reportType">报告类型枚举，有txt文件，xml文件，json文件，和自定义报告</param>
        /// <param name="customPrinter">可选参数，用于当reportType为custom的时候</param>
        public void PrintReport(string filePath, string runtimeHash, ReportType reportType, IResultPrinter customPrinter = null)
        {
            //检查文件路径正确与否
            string newFilePath = ModuleUtil.CheckFilePath(filePath);
            IResultPrinter _resultPrinter = null;
            switch (reportType)
            {
                case ReportType.txt:
                    _resultPrinter = new TxtWriter();
                    break;
                case ReportType.xml:
                    _resultPrinter = new XMLWriter();
                    break;
                case ReportType.json:
                    _resultPrinter = new JsonWriter();
                    break;
                case ReportType.custom:
                    //需输入继承IResultPrinter的customPrinter的实例
                    if (customPrinter == null)
                        //如果实例为null则抛出异常
                        throw new TestflowRuntimeException(ModuleErrorCode.CustomWriterNonExistent, "Custom ResultPrinter required.");
                    _resultPrinter = customPrinter;//继承IResultPrinter的自定义Printer
                    break;
            }

            _dataMaintainer.DesigntimeInitialize();
            //输出报告
            _resultPrinter.PrintReport(newFilePath, _dataMaintainer, runtimeHash);
        }

        public void Dispose()
        {
            _dataMaintainer?.Dispose();
        }
    }
}
