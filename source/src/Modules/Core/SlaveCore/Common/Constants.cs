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

        public const string TaskThreadNameFormt = "TaskThread:Session'{0}'Sequence'{1}'";

        public const string TaskRootThreadNameFormat = "TaskThread:Session'{0}'Root";

        public const string StatusMonitorThread = "MonitorThread:Session'{0}'";

        public const int WakeTimerInterval = 500;

        public const int KeyStatusSendInterval = 100;

        public const string EmptySetupName = "SetUp";
        public const string EmptyTeardownName = "TearDown";

        public const int MaxWatchDataLength = 65536;

        public const string IllegalValue = "N/A";
    }
}