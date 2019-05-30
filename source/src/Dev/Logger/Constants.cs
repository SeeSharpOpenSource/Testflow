namespace Testflow.Logger
{
    internal static class Constants
    {
        public const int DesigntimeSessionId = -1;
        public const int DefaultLogStreamSize = 8;
        public const string I18NName = "logger";
        public const string LogQueueName = @".\testflowlog\Journal$";

        public const string PlatformLogName = "PlatformLogger";
        public const string SlaveLogName = "SlaveLogger";

        public const string PlatformLogDir = @"Log\platform\";
        public const string SlaveLogDir = @"Log";

        public const string LogFilePostfix = ".log";

        public const string PlatformConfFile = @"deploy\platformlog.xml";
        public const string SlaveConfFile = @"deploy\slavelog.xml";

        public const string RootAppender = "RootAppender";
        public const string SlaveAppender = "SlaveAppender";
    }
}