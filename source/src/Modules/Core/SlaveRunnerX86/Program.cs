using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text;
using Testflow.SlaveCore;
using Testflow.Utility.Controls;

namespace Testflow.SlaveRunnerX86
{
    class Program
    {
        [HandleProcessCorruptedStateExceptions]
        static void Main(string[] args)
        {
            ProcessTestLauncher testLauncher = null;
            try
            {
                testLauncher = new ProcessTestLauncher(args[0]);
                testLauncher.Start();
            }
            catch (Exception ex)
            {
//                const string sourceName = "TestFlowSlaveRunner";
//                const string logName = "TestFlowSlaveLog";
//                if (!EventLog.SourceExists(sourceName))
//                {
//                    EventLog.CreateEventSource(sourceName, logName);
//                }
//                EventLog eventLog = new EventLog(logName) {Source = sourceName};
//                eventLog.WriteEntry(GetExceptionInfo(ex), EventLogEntryType.Error);
                ErrorInfoForm.ShowErrorInfoForm(Resource.ErrorHead, Resource.ErrorIntroduction, ex);
            }
            finally
            {
                testLauncher?.Dispose();
            }
        }

        private static string GetExceptionInfo(Exception ex)
        {
            StringBuilder errorInfo = new StringBuilder(2000);
            errorInfo.Append(ex.Message)
                .Append(Environment.NewLine)
                .Append("ErrorCode:")
                .Append(ex.HResult)
                .Append(Environment.NewLine)
                .Append(ex.StackTrace);
            if (null != ex.InnerException)
            {
                errorInfo.Append(Environment.NewLine).Append(GetExceptionInfo(ex.InnerException));
            }
            return errorInfo.ToString();
        }
    }
}
