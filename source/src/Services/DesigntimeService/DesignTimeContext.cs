using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.DesignTime;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.DesigntimeService
{
    class DesignTimeContext : IDesigntimeContext
    {
        public string Name { get; set; }

        public IDictionary<string, IComInterfaceDescription> Components { get; set; }

        public ISequenceGroup SequenceGroup { get; set; }

        public ITestProject TestProject { get; set; }

        public DesignTimeContext()
        {
            this.Name = string.Empty;
            this.Components = new Dictionary<string, IComInterfaceDescription>();
            this.SequenceGroup = new SequenceGroup();
            this.TestProject = null;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
