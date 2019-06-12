using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.EventData
{
    internal class TestProjectResults : List<ITestResultCollection>
    {
        public TestProjectResults(ISequenceFlowContainer sequenceData) : base(Constants.DefaultRuntimeSize)
        {
            if (sequenceData is ITestProject)
            {
                ITestProject testProject = (ITestProject)sequenceData;
                this.Add(new TestResultCollection(testProject));
                for (int i = 0; i < testProject.SequenceGroups.Count; i++)
                {
                    this.Add(new TestResultCollection(testProject.SequenceGroups[i], i));
                }
            }
            else
            {
                this.Add(new TestResultCollection((ISequenceGroup)sequenceData, 0));
            }
        }
    }
}