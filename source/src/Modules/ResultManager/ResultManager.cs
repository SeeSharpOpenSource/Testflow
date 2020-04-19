using Testflow.Modules;
using Testflow.Usr;
using Testflow.ResultManager.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Utility.I18nUtil;

namespace Testflow.ResultManager
{
    public class ResultManager : IResultManager
    {
        public ResultManager()
        {
            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_resultmanager_zh", "i18n_resultmanager_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
        }

        private IDataMaintainer _dataMaintainer;
        #region 初始化

        public IModuleConfigData ConfigData { get; set; }

        public void ApplyConfig(IModuleConfigData configData)
        {
            // TODO
        }

        public void DesigntimeInitialize()
        {
            if (_dataMaintainer == null)
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

        public void PrintReport(string filePath,  string runtimeHash, ReportType reportType,
            ISequenceFlowContainer sequenceData = null, IResultPrinter customPrinter = null)
        {
            //检查文件路径正确与否
            string newFilePath = ModuleUtil.CheckFilePath(filePath);
            IResultPrinter _resultPrinter = null;
            switch (reportType)
            {
                case ReportType.txt:
                    _resultPrinter = new TxtWriter(sequenceData);
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
            //输出报告
            _resultPrinter.PrintReport(newFilePath, sequenceData, runtimeHash);
        }

        public void Dispose()
        {
            _dataMaintainer?.Dispose();
        }
    }
}
