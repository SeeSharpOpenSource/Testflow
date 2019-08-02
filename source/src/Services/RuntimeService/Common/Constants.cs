using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testflow.RuntimeService
{
    internal static class Constants
    {
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
    }
}
