using System;

namespace Testflow.MasterCore.Common
{
    internal static class Constants
    {
        public const string LocalHostAddr = "127.0.0.1";
        public const int DefaultSequenceCapacaity = 10;
        public const int DefaultRuntimeSize = 10;
        public const string I18nName = "engineCore";
        public const int DefaultEventsQueueSize = 512;
        public const int MaxEventsQueueSize = 4096;

        public const int UnverifiedSequenceIndex = -1;

        public const int TestProjectSessionId = -1;
        public const int EventQueueTimeOut = 5000;

        public const int OperationTimeout = 1000;
        public const string TestProjectSessionName = "TestProjectSession";
        public const string AppDomainLauncherName = "Testflow.SlaveCore.AppDomainTestLauncher";
        public const string SlaveRunnerNameFormat = "TestFlowSlaveRunner_{0}";

        #region Block事件状态值定义

        public const int RmtGenState = 0;
        public const int WaitOverState = 1;
        public const int AbortState = 2;

        #endregion


        #region 事件名称

        public const string TestGenerationStart = "TestGenerationStart";
        public const string TestGenerationEnd = "TestGenerationEnd";
        public const string SessionGenerationStart = "SessionGenerationStart";
        public const string SessionGenerationReport = "SessionGenerationReport";
        public const string SessionGenerationEnd = "SessionGenerationEnd";
        public const string TestInstanceStart = "TestInstanceStart";
        public const string SessionStart = "SessionStart";
        public const string SequenceStarted = "SequenceStarted";
        public const string StatusReceived = "StatusReceived";
        public const string SequenceOver = "SequenceOver";
        public const string SessionOver = "SessionOver";
        public const string TestInstanceOver = "TestInstanceOver";
        public const string BreakPointHitted = "BreakPointHitted";

        #endregion

        #region 运行时对象常量定义

        public const long InvalidObjectId = -1;
        public const string BreakPointObjectName = "BreakPoint";
        public const string WatchDataObjectName = "WatchData";
        public const string EvaluationObjectName = "Evaluation";

        #endregion

        #region 运行时信息常量定义

        public const string RuntimeStateInfo = "RuntimeState";
        public const string ElapsedTimeInfo = "ElapsedTime";
        public const string RuntimeHashInfo = "RuntimeHash";
        public const string TestInstanceName = "TestName";
        public const string DebugHanle = "DebugHandle";

        #endregion


        public const int NoDebugHitSession = -1000;

        public const string X86SlaveRunnerFile = "SlaveRunner_x86.exe";
        public const string X64SlaveRunnerFile = "SlaveRunner_x64.exe";
    }
}
