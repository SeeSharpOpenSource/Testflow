namespace Testflow.EngineCore.Common
{
    internal static class Constants
    {
        public const int DefaultRuntimeSize = 10;
        public const string I18nName = "engineCore";

        public const int UnverifiedSequenceIndex = -1;

        public const int TestProjectSessionId = -1;


        #region 事件名称

        public const string TestGenerationStart = "TestGenerationStart";
        public const string TestGenerationReport = "TestGenerationReport";
        public const string TestGenerationEnd = "TestGenerationEnd";
        public const string SequenceStarted = "SequenceStarted";
        public const string StatusReceived = "StatusReceived";
        public const string SequenceOver = "SequenceOver";
        public const string TestOver = "TestOver";
        public const string BreakPointHitted = "BreakPointHitted";

        #endregion

    }
}
