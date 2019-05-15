using System;
using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.EventData
{
    public class TestGenerationInfo : ITestGenerationInfo
    {
        public IList<ISessionGenerationInfo> GenerationInfos { get; }
        public ISessionGenerationInfo RootGenerationInfo { get; }

        public TestGenerationInfo(ISequenceFlowContainer sequenceData)
        {
            if (sequenceData is ISequenceGroup)
            {
                this.GenerationInfos = new List<ISessionGenerationInfo>(1);
                this.GenerationInfos.Add(new SessionGenerationInfo((ISequenceGroup)sequenceData, 0));
                this.RootGenerationInfo = null;
            }
            else
            {
                ITestProject testProject = (ITestProject)sequenceData;
                this.RootGenerationInfo = new SessionGenerationInfo(testProject);
                this.GenerationInfos = new List<ISessionGenerationInfo>(Constants.DefaultRuntimeSize);
                for (int i = 0; i < testProject.SequenceGroups.Count; i++)
                {
                    this.GenerationInfos.Add(new SessionGenerationInfo(testProject.SequenceGroups[i], i));
                }
            }
        }

        public TestGenerationInfo(TestGenerationInfo generationInfo)
        {
            this.GenerationInfos = new List<ISessionGenerationInfo>(generationInfo.GenerationInfos.Count);
            foreach (ISessionGenerationInfo info in generationInfo.GenerationInfos)
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