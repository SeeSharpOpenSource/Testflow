using System;

namespace Testflow.SlaveCore.Common
{
    internal class LoopBreakException : ApplicationException
    {
        public LoopBreakAction BreakAction { get; }

        public LoopBreakException(LoopBreakAction breakAction, Exception innerException) : base("", innerException)
        {
            this.BreakAction = breakAction;
        }
    }
}