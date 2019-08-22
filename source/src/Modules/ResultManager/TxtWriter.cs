using System.Collections.Generic;
using System.IO;
using Testflow.Usr;
using Testflow.Modules;
using Testflow.Runtime.Data;
using Testflow.ResultManager.Common;
using Testflow.Data;

namespace Testflow.ResultManager
{
    //此类继承IResultPrinter用于打印到.txt文件
    internal class TxtWriter : IResultPrinter
    {
        internal IDataMaintainer _dataMaintainer;

        internal TxtWriter()
        {
            _dataMaintainer = TestflowRunner.GetInstance().DataMaintainer;
        }

        //父类继承
        public void PrintReport(string filePath, string runtimeHash)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filePath, append: true);
                //PrintSessionResults()
                PrintTestInstance(sw, runtimeHash);
            }
            catch (IOException ex)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.IOError, ex.Message, ex);
            }
            finally
            {
                sw.Close();
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
            try
            {
                if(testInstance != null)
                {
                    sw.WriteLine(testInstance.Name);
                    sw.WriteLine(testInstance.Description);
                    sw.WriteLine(testInstance.TestProjectName);
                    sw.WriteLine(testInstance.TestProjectDescription);
                    sw.WriteLine($"StartGenTime: {testInstance.StartGenTime}");
                    sw.WriteLine($"EndGenTime: {testInstance.EndGenTime}");
                    sw.WriteLine($"StartTime: {testInstance.StartTime}");
                    sw.WriteLine($"End Time: {testInstance.EndTime}");
                    sw.WriteLine($"Elapsed Time: {testInstance.ElapsedTime}");
                    sw.WriteLine();
                }

                //输出报告每个session
                PrintSessionResults(sw, runtimeHash);            }
            catch (IOException ex)
            {
                sw.Close();
                throw new TestflowRuntimeException(ModuleErrorCode.IOError, "PrintTestInstance IO Exception", ex);
            }
            
        }

        private void PrintSessionResults(StreamWriter sw, string runtimeHash)
        {
            IList<SessionResultData> sessionResultList = _dataMaintainer.GetSessionResults(runtimeHash);
            try
            {
                //List中循环每个session
                foreach (SessionResultData sessionResult in sessionResultList)
                {
                    sw.WriteLine(sessionResult.Name);
                    sw.WriteLine(sessionResult.Description);
                    int sessionId = sessionResult.Session;
                    sw.WriteLine($"Session ID: {sessionId}");
                    sw.WriteLine($"StartTime: {sessionResult.StartTime}");
                    sw.WriteLine($"EndTime: {sessionResult.EndTime}");
                    sw.WriteLine($"Elapsed Time: {sessionResult.ElapsedTime}");
                    sw.WriteLine($"State: {sessionResult.State}");
                    if (sessionResult.State == Runtime.RuntimeState.Failed || sessionResult.State == Runtime.RuntimeState.Error)
                    {
                        sw.WriteLine($"FailedInfo: {sessionResult.FailedInfo}");
                    }
                    sw.WriteLine();

                    //输出报告每个Sequence
                    PrintSequenceResults(sw, runtimeHash, sessionId);
                    //输出报告此session的Performance
                    PrintPerformance(sw, runtimeHash, sessionId);
                }
            }
            catch (IOException ex)
            {
                sw.Close();
                throw new TestflowRuntimeException(ModuleErrorCode.IOError, "PrintSessionResults IO Exception", ex);
            }
        }

        private void PrintSequenceResults(StreamWriter sw, string runtimeHash, int sessionId)
        {
            IList<SequenceResultData> sequenceResultList = _dataMaintainer.GetSequenceResults(runtimeHash, sessionId);
            try
            {
                //List中循环每个sequence
                foreach (SequenceResultData sequenceResult in sequenceResultList)
                {
                    sw.WriteLine(sequenceResult.Name);
                    sw.WriteLine(sequenceResult.Description);
                    sw.WriteLine($"Session ID: {sequenceResult.Session}");
                    sw.WriteLine($"Sequence Index: {sequenceResult.SequenceIndex}");
                    sw.WriteLine($"Sequence Result: {sequenceResult.Result.ToString()}");
                    sw.WriteLine($"StartTime: {sequenceResult.StartTime}");
                    sw.WriteLine($"EndTime: {sequenceResult.EndTime}");
                    sw.WriteLine($"ElapsedTime: {sequenceResult.ElapsedTime}");
                    if (sequenceResult.Result == Runtime.RuntimeState.Failed)
                    {
                        sw.WriteLine($"FailInfo: {sequenceResult.FailInfo}");
                        sw.WriteLine($"FailStack: {sequenceResult.FailStack}");
                    }
                    sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                sw.Close();
                throw new TestflowRuntimeException(ModuleErrorCode.IOError, "PrintSequenceResults IO Exception", ex);
            }
        }

        /// <summary>
        /// 打印一个session（会话序列组）的performance
        /// 打印序列processor time极值
        /// 打印序列memory used极值、均值
        /// 实际计算方法在ResultManager.Common.ModuleUtil类
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="dataMaintainer"></param>
        /// <param name="runtimeHash"></param>
        /// <param name="sessionId"> 会话id </param>
        private void PrintPerformance(StreamWriter sw, string runtimeHash, int sessionId)
        {
            IList<PerformanceStatus> performanceList = _dataMaintainer.GetPerformanceStatus(runtimeHash, sessionId);
            if (performanceList.Count == 0)
            {
                return;
            }
            try
            {
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
            catch (IOException ex)
            {
                sw.Close();
                throw new TestflowRuntimeException(ModuleErrorCode.IOError, "PrintPerformance IO Exception", ex);
            }
        }

    }
}
