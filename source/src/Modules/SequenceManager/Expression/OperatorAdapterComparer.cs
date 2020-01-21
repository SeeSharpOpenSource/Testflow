using System.Collections.Generic;

namespace Testflow.SequenceManager.Expression
{
    internal class OperatorAdapterComparer : IComparer<OperatorAdapter>
    {
        // 从打到小排序
        public int Compare(OperatorAdapter x, OperatorAdapter y)
        {
            if (x.Priority == y.Priority)
            {
                return 0;
            }
            return x.Priority > y.Priority ? 1 : -1;
        }
    }
}