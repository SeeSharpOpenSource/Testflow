namespace Testflow.DataMaintainer
{
    internal static class DataBaseItemNames
    {
        #region Table names

        public const string InstanceTableName = "Testflow_TestInstances";
        public const string SessionTableName = "Testflow_SessionResults";
        public const string SequenceTableName = "Testflow_SequenceResults";
        public const string StatusTableName = "Testflow_RuntimeStatusDatas";
        public const string PerformanceTableName = "Testflow_PerformanceDatas";

        #endregion


        #region Column names

        public const string RuntimeIdColumn = "RuntimeHash";
        public const string InstanceNameColumn = "InstanceName";
        public const string SequenceGroupNameColumn = "SequenceGroupName";
        public const string NameColumn = "Name";
        public const string DescriptionColumn = "Description";
        public const string ProjectNameColumn = "TestProjectName";
        public const string ProjectDescriptionColumn = "TestProjectDescription";
        public const string StartGenTimeColumn = "StartGenTime";
        public const string EndGenTimeColumn = "EndGenTime";
        public const string StartTimeColumn = "StartTime";
        public const string EndTimeColumn = "EndTime";
        public const string ElapsedTimeColumn = "ElapsedTime";
        public const string SessionIdColumn = "SessionId";
        public const string SequenceHashColumn = "SequenceHash";
        public const string SessionStateColumn = "SessionState";
        public const string FailedInfoColumn = "FailedInfo";
        public const string SequenceIndexColumn = "SequenceIndex";
        public const string SequenceResultColumn = "SequenceResult";
        public const string FailStackColumn = "FailStack";
        public const string StatusIndexColumn = "StatusIndex";
        public const string RecordTimeColumn = "RecordTime";
        public const string StepResultColumn = "StepResult";
        public const string StackColumn = "Stack";
        public const string WatchDataColumn = "WatchData";
        public const string MemoryUsedColumn = "MemoryUsed";
        public const string MemoryAllocatedColumn = "MemoryAllocated";
        public const string ProcessorTimeColumn = "ProcessorTime";

        #endregion

    }
}