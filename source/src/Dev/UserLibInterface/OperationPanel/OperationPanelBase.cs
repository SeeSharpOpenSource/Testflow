using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ExtensionBase.OperationPanel
{
    public abstract class OperationPanelBase
    {
        public ISequenceFlowContainer SequenceData { get; }

        public RunnerType RunnerType { get; }

        protected OperationPanelBase(ISequenceFlowContainer sequenceData)
        {
            I18NOption i18NOption = new I18NOption(Assembly.GetAssembly(this.GetType()), "i18n_userlib_zh",
                "i18n_userlib_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N i18N = I18N.GetInstance(Constants.I18nName);

            if (sequenceData is ITestProject)
            {
                RunnerType = RunnerType.TestProject;
            }
            else if (sequenceData is ISequenceGroup)
            {
                RunnerType = RunnerType.SequenceGroup;
            }
            else
            {
                throw new TestflowDataException(-1, i18N.GetStr("InvalidSequence"));
            }
            this.SequenceData = sequenceData;
        }

        public abstract void TestGenerationStart(ITestGenerationInfo generationInfo);
        public abstract void TestGenerationOver(ITestGenerationInfo generationInfo);
        public abstract void TestGenerationFailed(ITestGenerationInfo generationInfo);
        public abstract void TestStart(DateTime startTime);
        public abstract void SessionStart(int session, DateTime time);
        public abstract void SequenceStart(ISequence sequence, DateTime time);
        public abstract void StatusReceived(ISequenceStep currentStep, IRuntimeStatusInfo runtimeInfo);
        public abstract void SequenceOver(ISequenceTestResult sequenceResult);
        public abstract void SessionOver(ITestResultCollection sessionResult);
        public abstract void TestOver(IList<ITestResultCollection> testResults);
        public abstract void ErrorOccured(ISequenceStep errorStep, IFailedInfo failedInfo);
    }
}