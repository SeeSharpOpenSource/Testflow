namespace Testflow.SlaveCore.Common
{
    internal static class Constants
    {
        public const int DefaultRuntimeSize = 10;
        public const int UnverifiedSequenceIndex = -1;

        public const string I18nName = "SlaveCore";

        public const string DefaultNumericFormat = "0.######";

        public const string StructTypeName = "Struct";

        public const string PropertyDelim = ".";

        public const int ThreadAbortJoinTime = 10000;

        public const int AbortWaitTime = 200;

        public const int ExecutionTrackerSize = 500;

        public const string TaskThreadNameFormt = "TaskThread:Session'{0}'Sequence'{1}'";

        public const string TaskRootThreadNameFormat = "TaskThread:Session'{0}'Root";

        public const string StatusMonitorThread = "MonitorThread:Session'{0}'";

        public const int WakeTimerInterval = 500;

        public const int KeyStatusSendInterval = 100;

        public const string EmptySetupName = "SetUp";
        public const string EmptyTeardownName = "TearDown";

        public const int MaxWatchDataLength = 65536;

        public const string IllegalValue = "N/A";

        public const int MaxRmtMessageCount = 20;

        #region 表达式相关

        public const string ArgNameFormat = "ARG{0}";
        public const string ArgNamePattern = "(?:(?:ARG)|(?:EXP))\\d+";
        public const string ExpPlaceHodlerPattern = "EXP\\d+";
        public const string ExpPlaceHodlerFormat = "EXP{0}";
        public const string SingleArgPattern = "^ARG\\d+$";
        public const string SingleExpPattern = "^EXP\\d+$";
        public const string DigitPattern = "^(\\+|-)?(?:\\d+(?:\\.\\d+)?|0x[0-9a-fA-F]+|\\d+(?:\\.\\d+)?[Ee](?:[\\+-]?\\d+))?$";
        public const string StringPattern = "^(\"|')(.*)\\1$";
        public const string BoolPattern = "^(?:[Tt]rue|[Ff]alse)$";

        #endregion
    }
}