namespace Testflow.CoreCommon.Common
{
    public static class MessageNames
    {
        public const string CtrlAbort = "Abort";
        public const string CtrlStart = "Start";

        #region 调试消息名

        public const string AddBreakPointName = "AddBreakPoint";
        public const string DelBreakPointName = "DeleteBreakPoint";
        public const string BreakPointHitName = "BreakPointHitted";
        public const string StepOverName = "StepOver";
        public const string StepIntoName = "StepInto";
        public const string ContinueName = "Continue";
        public const string RunToEndName = "RunToEnd";
        public const string RequestValueName = "RequestValue";
        public const string RefreshWatchName = "RefreshWatch";
        public const string PauseName = "Pause";

        #endregion


        public const string DownRmtGenMsgName = "RemoteGen";
        public const string UpRmtGenMsgName = "RemoteGen";

        #region 状态消息名

        public const string StartStatusName = "Start";
        public const string ReportStatusName = "Report";
        public const string ResultStatusName = "Result";
        public const string ErrorStatusName = "Error";
        public const string HeartBeatStatusName = "HeartBeat";

        #endregion


        public const string TestGenName = "TestGen";

        public const string CallBackName = "CallBack";
    }
}