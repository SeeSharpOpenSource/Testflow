using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Testflow.Usr;
using Testflow.Modules;
using Testflow.Runtime.Data;
using Testflow.ResultManager.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.Utils;

namespace Testflow.ResultManager
{
    //此类继承IResultPrinter用于打印到.txt文件
    internal class TxtWriter : IResultPrinter
    {
        private IDataMaintainer _dataMaintainer;
        private readonly I18N _i18n;
        const string DateFormat = "yyyy-MM-dd hh:mm:ss.fff";
        const string DoubleFormat = "F3";
        const int TabLength = 8;
        private ISequenceFlowContainer _sequenceData;

        internal TxtWriter(ISequenceFlowContainer sequenceData)
        {
            sequenceData = sequenceData;
            _dataMaintainer = TestflowRunner.GetInstance().DataMaintainer;
            _i18n = I18N.GetInstance(Constants.I18nName);
        }

        //父类继承
        public void PrintReport(string filePath, ISequenceFlowContainer sequenceData, string runtimeHash)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filePath, true);
                //PrintSessionResults()
                PrintTestInstance(sw, runtimeHash);
            }
            catch (IOException ex)
            {
                TestflowRunner.GetInstance().
                    LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, ex.Message);
                throw new TestflowRuntimeException(ModuleErrorCode.IOError, ex.Message, ex);
            }
            finally
            {
                sw?.Close();
            }
        }

        //父类继承
        //public void PrintReport(string filePath, string runtimeHash, int sessionId)
        //{

        //}

        /*
         * 已下输出方法结构类似，采用嵌套循环：
         * 在PrintTestInstance里调用PrintSessionResults；
         * 在PrintSessionResults里调用PrintSequenceResults和PrintPerformance；
         */
        private void PrintTestInstance(StreamWriter sw, string runtimeHash)
        {
            TestInstanceData testInstance = _dataMaintainer.GetTestInstance(runtimeHash);
            if (testInstance != null)
            {
                WriteRecord(sw, 0, "TestInstanceName", testInstance.Name);
                WriteRecord(sw, 0, "TestInstanceDescription", testInstance.Description);
                WriteRecord(sw, 0, "TestProjectName", testInstance.TestProjectName);
                WriteRecord(sw, 0, "TestProjectDescription", testInstance.TestProjectDescription);
                WriteRecord(sw, 0, "StartGenTime", testInstance.StartGenTime.ToString(DateFormat));
                WriteRecord(sw, 0, "EndGenTime", testInstance.EndGenTime.ToString(DateFormat));
                WriteRecord(sw, 0, "StartTime", testInstance.StartTime.ToString(DateFormat));
                WriteRecord(sw, 0, "EndTime", testInstance.EndTime.ToString(DateFormat));
                WriteRecord(sw, 0, "ElapsedTime", (testInstance.ElapsedTime / 1000).ToString(DoubleFormat));
                sw.WriteLine();
            }
            //输出报告每个session
            PrintSessionResults(sw, runtimeHash);
        }

        private void PrintSessionResults(StreamWriter sw, string runtimeHash)
        {
            IList<SessionResultData> sessionResultList = _dataMaintainer.GetSessionResults(runtimeHash);
            //List中循环每个session
            foreach (SessionResultData sessionResult in sessionResultList)
            {
                WriteRecord(sw, 1, "SequenceGroupName", sessionResult.Name);
                WriteRecord(sw, 1, "SequenceGroupDescription", sessionResult.Description);
                WriteRecord(sw, 1, "SessionId", sessionResult.Session.ToString());
                WriteRecord(sw, 1, "StartTime", sessionResult.StartTime.ToString(DateFormat));
                WriteRecord(sw, 1, "EndTime", sessionResult.EndTime.ToString(DateFormat));
                WriteRecord(sw, 1, "ElapsedTime", (sessionResult.ElapsedTime / 1000).ToString(DoubleFormat));
                WriteRecord(sw, 1, "SessionResult", sessionResult.State.ToString());
                if (sessionResult.State == Runtime.RuntimeState.Failed || sessionResult.State == Runtime.RuntimeState.Error)
                {
                    WriteRecord(sw, 1, "FailedInfo", sessionResult.FailedInfo.Message);
                }

                sw.WriteLine();

                //输出报告每个Sequence
                PrintSequenceResults(sw, runtimeHash, sessionResult.Session);
                //输出报告此session的Performance
                PrintPerformance(sw, runtimeHash, sessionResult.Session);
            }
        }

        private void PrintSequenceResults(StreamWriter sw, string runtimeHash, int sessionId)
        {
            IList<SequenceResultData> sequenceResultList = _dataMaintainer.GetSequenceResults(runtimeHash, sessionId);
            //List中循环每个sequence
            SequenceResultData teardownResult = sequenceResultList.FirstOrDefault(item => item.SequenceIndex == CommonConst.TeardownIndex);
            sequenceResultList.Remove(teardownResult);
            sequenceResultList.Add(teardownResult);
            foreach (SequenceResultData sequenceResult in sequenceResultList)
            {
                WriteRecord(sw, 2, "SequenceName", sequenceResult.Name);
                WriteRecord(sw, 2, "SequenceDescription", sequenceResult.Description);
                WriteRecord(sw, 2, "SequenceIndex", sequenceResult.SequenceIndex.ToString());
                WriteRecord(sw, 2, "SequenceResult", sequenceResult.Result.ToString());
                WriteRecord(sw, 2, "StartTime", sequenceResult.StartTime.ToString(DateFormat));
                WriteRecord(sw, 2, "EndTime", sequenceResult.EndTime.ToString(DateFormat));
                WriteRecord(sw, 2, "ElapsedTime", (sequenceResult.ElapsedTime / 1000).ToString(DoubleFormat));
                if (sequenceResult.Result == Runtime.RuntimeState.Failed && null != sequenceResult.FailInfo)
                {
                    WriteRecord(sw, 2, "FailedInfo", sequenceResult.FailInfo.Message);
                    WriteRecord(sw, 2, "FailedStack", sequenceResult.FailStack);
                }
                PrintRuntimeStatus(sw, runtimeHash, sessionId, sequenceResult.SequenceIndex);
                sw.WriteLine();
            }
        }

        private void PrintRuntimeStatus(StreamWriter sw, string runtimeHash, int sessionId, int sequenceIndex)
        {
            const int stepNameLength = 3;
            const int recordTimeLength = 4;
            const int resultLength = 2;
            const int failInfoLength = 5;
            const string preOffset = "\t\t";

            IList<RuntimeStatusData> runtimeStatus = _dataMaintainer.GetRuntimeStatus(runtimeHash, sessionId, sequenceIndex);
            if (runtimeStatus.Count > 1)
            {
                sw.WriteLine(preOffset + _i18n.GetStr("RuntimeStatusName"));
                StringBuilder tableContent = new StringBuilder(300);
                string stepNameTitle = _i18n.GetStr("StepNameTitle");
                string recordTimeTitle = _i18n.GetStr("RecordTimeTitle");
                string resultTitle = _i18n.GetStr("StepResultTitle");
                string failInfoTitle = _i18n.GetStr("FailedInfoTitle");

                tableContent.Append(preOffset).Append(stepNameTitle).Append(GetDelimTab(stepNameTitle, stepNameLength));
                tableContent.Append(recordTimeTitle).Append(GetDelimTab(recordTimeTitle, recordTimeLength));
                tableContent.Append(resultTitle).Append(GetDelimTab(resultTitle, resultLength));
                tableContent.Append(failInfoTitle).Append(GetDelimTab(failInfoTitle, failInfoLength));

                sw.WriteLine(tableContent.ToString());
                int titleLength = Encoding.GetEncoding("GBK").GetByteCount(tableContent.ToString());
                titleLength += 30;
                tableContent.Clear();
                tableContent.Append(preOffset);
                for (int i = 0; i < titleLength - 2; i++)
                {
                    tableContent.Append("-");
                }
                sw.WriteLine(tableContent.ToString());
                for (int i = 0; i < runtimeStatus.Count - 1; i++)
                {
                    tableContent.Clear();
                    tableContent.Append(preOffset);
                    string stackStr = runtimeStatus[i].Stack;
                    string stepName = GetStepName(stackStr);
                    string time = runtimeStatus[i].Time.ToString(DateFormat);
                    string result = runtimeStatus[i].Result.ToString();
                    string failInfo = runtimeStatus[i].FailedInfo?.Message ?? string.Empty;
                    tableContent.Append(stepName).Append(GetDelimTab(stepName, stepNameLength));
                    tableContent.Append(time).Append(GetDelimTab(time, recordTimeLength));
                    tableContent.Append(result).Append(GetDelimTab(result, resultLength));
                    tableContent.Append(failInfo).Append(GetDelimTab(stepName, failInfoLength));
                    sw.WriteLine(tableContent.ToString());
                }
                sw.WriteLine("");
            }
        }

        private string GetStepName(string stackStr)
        {
            ISequenceStep step = null;
            if (_sequenceData is ISequenceGroup)
            {
                step = SequenceUtils.GetStepFromStack((ISequenceGroup) _sequenceData, stackStr);
            }
            else if (_sequenceData is ITestProject)
            {
                step = SequenceUtils.GetStepFromStack((ITestProject) _sequenceData, stackStr);
            }
            return step?.Name ?? stackStr;
        }

        private string GetDelimTab(string printText, int maxTabCount)
        {
            int labelShowLength = Encoding.GetEncoding("GBK").GetByteCount(printText);
            int delimCount = (int)Math.Ceiling(((double)maxTabCount*TabLength - labelShowLength) / 8);
            StringBuilder delims = new StringBuilder(5);
            if (delimCount <= 0 && maxTabCount * TabLength < labelShowLength)
            {
                delimCount = 1;
            }
            for (int i = 0; i < delimCount; i++)
            {
                delims.Append("\t");
            }
            return delims.ToString();
        }

        /// <summary>
        /// 打印一个session（会话序列组）的performance
        /// 打印序列processor time极值
        /// 打印序列memory used极值、均值
        /// 实际计算方法在ResultManager.Common.ModuleUtil类
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="runtimeHash"></param>
        /// <param name="sessionId"> 会话id </param>
        private void PrintPerformance(StreamWriter sw, string runtimeHash, int sessionId)
        {
            IList<PerformanceStatus> performanceList = _dataMaintainer.GetPerformanceStatus(runtimeHash, sessionId);
            if (performanceList.Count == 0)
            {
                return;
            }
            //大小为2的数组记录极值
            double[] mmpt = ModuleUtil.getMaxMinProcessorTime(performanceList);
            sw.WriteLine("Max Processor Time: {0}", mmpt[0]);
            sw.WriteLine("Min Processor Time: {0}", mmpt[1]);
            //大小为3的数组记录极值、均值
            long[] mmamu = ModuleUtil.getMaxMinAveMemoryUsed(performanceList);
            sw.WriteLine("Max Memory Used: {0}", mmamu[0]);
            sw.WriteLine("Min Memory Used: {0}", mmamu[1]);
            sw.WriteLine("Ave Memory Used: {0}", mmamu[2]);
            sw.WriteLine();
        }

        private void WriteRecord(StreamWriter writer, int level, string labelKey, string value)
        {
            const int valueStartOffset = TabLength*3;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            string label = _i18n.GetStr(labelKey);
            StringBuilder recordString = new StringBuilder(100);
            for (int i = 0; i < level; i++)
            {
                recordString.Append("\t");
            }
            recordString.Append(label);
            int labelShowLength = Encoding.GetEncoding("GBK").GetByteCount(label);
            int delimCount = (int) Math.Ceiling(((double)valueStartOffset - labelShowLength) / 8);
            for (int i = 0; i < delimCount; i++)
            {
                recordString.Append("\t");
            }
            recordString.Append(value);
            writer.WriteLine(recordString);
        }
    }
}
