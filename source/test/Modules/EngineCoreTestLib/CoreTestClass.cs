using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EngineCoreTestLib
{
    public class CoreTestClass
    {
        private int _waitTime;

        public static void StaticWait(int time)
        {
            Thread.Sleep(time);
        }

        public CoreTestClass(int waitTime)
        {
            _waitTime = waitTime;
        }

        public int WaitTime(int extraWaitTime)
        {
            int totalWaitTime = extraWaitTime + _waitTime;
            Thread.Sleep(totalWaitTime);
            return totalWaitTime;
        }

        public void RaiseError()
        {
            throw new ApplicationException("This is just an exception test.");
        }

        public void GetWaitTime(out int waitTimeValue)
        {
            waitTimeValue = _waitTime;
        }
    }
}
