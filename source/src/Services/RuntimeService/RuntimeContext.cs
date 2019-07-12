using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.RuntimeService
{
    public class RuntimeContext : IRuntimeContext
    {
        public string Name { get; }

        public long SessionId { get; }

        public ITestProject TestGroup { get; }

        public ISequenceGroup SequenceGroup { get; }

        public IHostInfo HostInfo { get; }

        public Process Process { get; }

        public AppDomain RunDomain { get; set; }

        public int ThreadID { get; }

        public ISequenceDebuggerCollection Debuggers { get; }

        public IDebuggerHandle DebuggerHandle { get; }

        public IRuntimeStatusCollection RunTimeStatus { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object GetService(string serviceName, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
