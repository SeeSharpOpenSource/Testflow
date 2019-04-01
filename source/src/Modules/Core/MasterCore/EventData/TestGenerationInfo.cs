using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;

namespace Testflow.MasterCore.EventData
{
    public class TestGenerationInfo : ITestGenerationInfo
    {
        public IList<ISequenceGenerationInfo> GenerationInfos { get; }
        public ISequenceGenerationInfo RootGenerationInfo { get; }

        public TestGenerationInfo(ISequenceFlowContainer sequenceData)
        {
            if (sequenceData is ISequenceGroup)
            {
                this.GenerationInfos = new List<ISequenceGenerationInfo>(1);
                this.GenerationInfos.Add(new SequenceGenerationInfo((ISequenceGroup)sequenceData, 0));
                this.RootGenerationInfo = null;
            }
            else
            {
                ITestProject testProject = (ITestProject)sequenceData;
                this.RootGenerationInfo = new SequenceGenerationInfo(testProject);
                this.GenerationInfos = new List<ISequenceGenerationInfo>(Constants.DefaultRuntimeSize);
                for (int i = 0; i < testProject.SequenceGroups.Count; i++)
                {
                    this.GenerationInfos.Add(new SequenceGenerationInfo(testProject.SequenceGroups[i], i));
                }
            }
        }

        public TestGenerationInfo(TestGenerationInfo generationInfo)
        {
            this.GenerationInfos = new List<ISequenceGenerationInfo>(generationInfo.GenerationInfos.Count);
            foreach (ISequenceGenerationInfo info in generationInfo.GenerationInfos)
            {
                this.GenerationInfos.Add(info.Clone());
            }
            this.RootGenerationInfo = generationInfo.RootGenerationInfo.Clone();
        }

        public ITestGenerationInfo Clone()
        {
            return new TestGenerationInfo(this);
        }
    }
}