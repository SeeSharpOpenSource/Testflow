namespace Testflow.SlaveCore.Common
{
    internal static class Constants
    {
        public const int DefaultRuntimeSize = 10;
        public const int UnverifiedSequenceIndex = -1;

        public const string I18nName = "SlaveCore";

        public const string PropertyDelim = ".";

        public const int ThreadAbortJoinTime = 10000;

        public const string TaskThreadNameFormt = "TaskThread:Session'{0}'Sequence'{1}'";

        public const string TaskRootThreadNameFormat = "TaskThread:Session'{0}'Root";

        public const string StatusMonitorThread = "MonitorThread:Session'{0}'";

        public const int WakeTimerInterval = 500;

        public const int KeyStatusSendInterval = 100;
    }
}