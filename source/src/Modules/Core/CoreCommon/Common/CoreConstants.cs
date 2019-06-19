namespace Testflow.CoreCommon.Common
{
    public static class CoreConstants
    {
        public const string LocalHostAddr = "127.0.0.1";
        public const int DefaultSequenceCapacaity = 10;
        public const int DefaultRuntimeSize = 5;
        public const string I18nName = "engineCore";
        public const int DefaultEventsQueueSize = 512;
        public const int MaxEventsQueueSize = 12800;

        public const int UnverifiedSequenceIndex = -1;

        public const int TestProjectIndex = -1;

        public const int TestProjectSessionId = -1;
        public const int EventQueueTimeOut = 500;

        public const int OperationTimeout = 1000;

        public const string UpLinkMQName = @".\Private$\TestflowUpLinkQueue";
        public const string DownLinkMQName = @".\Private$\TestflowDownLinkQueue";

        public const string NullValue = "NULL";

        public const string ErrorVarValue = "InvalidValue";

        public const int EmptyStepIndex = 0;
    }
}