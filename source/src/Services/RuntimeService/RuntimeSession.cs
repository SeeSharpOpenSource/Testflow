using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.RuntimeService
{
    public class RuntimeSession : IRuntimeSession
    {
        private Modules.IEngineController _engineController;

        public IRuntimeContext Context { get; }

        public RuntimeState State { get; }

        public int ID { get; }

        #region 初始化
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 开始、结束运行 //toask 这个start and stop该做什么
        public void Start()
        {
            _engineController.Start();
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 事件相关
        public event RuntimeDelegate.SessionGenerationAction SessionGenerationStart;
        public event RuntimeDelegate.SessionGenerationAction SessionGenerationReport;
        public event RuntimeDelegate.SessionGenerationAction SessionGenerationEnd;
        public event RuntimeDelegate.SessionStatusAction SessionStart;
        public event RuntimeDelegate.SequenceStatusAction SequenceStarted;
        public event RuntimeDelegate.StatusReceivedAction StatusReceived;
        public event RuntimeDelegate.SequenceStatusAction SequenceOver;
        public event RuntimeDelegate.SessionStatusAction SessionOver;
        public event RuntimeDelegate.BreakPointHittedAction BreakPointHitted;

        public void RuntimeSessionRegister(Delegate callBack, string eventName)
        {
            _engineController.RegisterRuntimeEvent(callBack, eventName, CoreCommon.Common.CoreConstants.TestProjectSessionId);
        }

        private void OnSessionGenerationStart(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationStart?.Invoke(generationinfo);
        }

        private void OnSessionGenerationReport(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationReport?.Invoke(generationinfo);
        }

        private void OnSessionGenerationEnd(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationEnd?.Invoke(generationinfo);
        }

        private void OnTestStart(ITestResultCollection statistics)
        {
            SessionStart?.Invoke(statistics);
        }

        private void OnSequenceStarted(ISequenceTestResult result)
        {
            SequenceStarted?.Invoke(result);
        }

        private void OnStatusReceived(IRuntimeStatusInfo statusinfo)
        {
            StatusReceived?.Invoke(statusinfo);
        }

        private void OnSequenceOver(ISequenceTestResult result)
        {
            SequenceOver?.Invoke(result);
        }

        private void OnTestOver(ITestResultCollection statistics)
        {
            SessionOver?.Invoke(statistics);
        }

        private void OnBreakPointHitted(IDebuggerHandle debuggerHandle, IDebugInformation information)
        {
            BreakPointHitted?.Invoke(debuggerHandle, information);
        }

        #endregion

        #region Get Sequence, SequenceStep
        public ISequence GetSequence(int index)
        {
            return Context.SequenceGroup.Sequences[index];
        }

        //to do: check valid steps
        public ISequenceStep GetSequenceStep(ICallStack stack)
        {
            ISequence sequence = Context.SequenceGroup.Sequences[stack.Sequence];
            ISequenceStep step = sequence.Steps[stack.StepStack[0]];
            for (int n=1; n < stack.StepStack.Count; n++)
            {
                step = step.SubSteps[stack.StepStack[n]];
            }
            return step;
        }
        #endregion

        #region 断点设置
        public bool HasBreakPoint(ISequence sequence, ISequenceStep sequenceStep)
        {

            throw new NotImplementedException();
        }

        public bool AddBreakPoint(ISequence sequence, ISequenceStep sequenceStep)
        {
            throw new NotImplementedException();
        }
        public void RemoveAllBreakPoint(ISequence sequence)
        {
            throw new NotImplementedException();
        }

        public bool RemoveBreakPoint(ISequence sequence, ISequenceStep sequenceStep)
        {
            throw new NotImplementedException();
        }
        #endregion


        public void ForceRefreshStatus()
        {
            throw new NotImplementedException();
        }

    }
}
