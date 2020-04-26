using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Modules;

namespace Testflow.RuntimeService
{
    public class RuntimeSession : IRuntimeSession
    {
        private IEngineController _engineController;

        public IRuntimeContext Context { get; }

        public RuntimeState State { get { return _engineController.GetRuntimeState(ID); } }

        public int ID { get; }

        public RuntimeSession(int id, IRuntimeContext context)
        {
            this.ID = id;
            this.Context = context;
        }


        #region 初始化
        public void Initialize()
        {
            _engineController = TestflowRunner.GetInstance().EngineController;
            
        }

        public void Dispose()
        {

        }
        #endregion

        #region 开始引擎
        //todo I18n
        public void Start()
        {
            RegisterEvents();

            if (Context.SequenceGroup == null)
            {
                if(Context.TestGroup == null)
                {
                    throw new TestflowException(ModuleErrorCode.TestProjectDNE, "Neither Test Project nor Sequence Group exists");
                }
                throw new TestflowException(ModuleErrorCode.SequenceGroupDNE, "Sequence Group does not exist; please load using RuntimeService");
            }

            ModuleUtils.EngineStartThread(Context.SequenceGroup);
        }

       

        public void Stop()
        {
            _engineController.Stop();
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

        //想engineController.runtimeEngine注册添加好的事件
        private void RegisterEvents()
        {
            //注册事件,空事件在runtimeEngine方法里面判断
            _engineController.RegisterRuntimeEvent(SessionGenerationStart, CommonConst.BroadcastSession, Constants.SessionGenerationStart, Context.SessionId);
            _engineController.RegisterRuntimeEvent(SessionGenerationReport, CommonConst.BroadcastSession, Constants.SessionGenerationReport, Context.SessionId);
            _engineController.RegisterRuntimeEvent(SessionGenerationEnd, CommonConst.BroadcastSession, Constants.SessionGenerationEnd, Context.SessionId);
            _engineController.RegisterRuntimeEvent(SessionStart, CommonConst.BroadcastSession, Constants.SessionStart, Context.SessionId);
            _engineController.RegisterRuntimeEvent(SequenceStarted, CommonConst.BroadcastSession, Constants.SequenceStarted, Context.SessionId);
            _engineController.RegisterRuntimeEvent(StatusReceived, CommonConst.BroadcastSession, Constants.StatusReceived, Context.SessionId);
            _engineController.RegisterRuntimeEvent(SequenceOver, CommonConst.BroadcastSession, Constants.SequenceOver, Context.SessionId);
            _engineController.RegisterRuntimeEvent(SessionOver, CommonConst.BroadcastSession, Constants.SessionOver, Context.SessionId);
            _engineController.RegisterRuntimeEvent(BreakPointHitted, CommonConst.BroadcastSession, Constants.BreakPointHitted, Context.SessionId);
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
