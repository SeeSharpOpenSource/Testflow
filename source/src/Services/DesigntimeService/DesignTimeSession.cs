using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.DesignTime;
using Testflow.Modules;
using Testflow.DesigntimeService.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.DesigntimeService
{
    public class DesignTimeSession : IDesignTimeSession
    {
        private Modules.ISequenceManager _sequenceManager => TestflowRunner.GetInstance().SequenceManager;
        private Modules.IComInterfaceManager _interfaceManager => TestflowRunner.GetInstance().ComInterfaceManager;

        public long SessionId { get ; set ; }

        private IDesigntimeContext _contextInst;
        public IDesigntimeContext Context
        {
            get { return _contextInst;}
        }

        private void ContextSequenceGroupModify()
        {
            Context.SequenceGroup.Info.Modified = true;
            Context.SequenceGroup.Info.ModifiedTime = DateTime.Now;
        }
       
        public DesignTimeSession(long sessionId, ISequenceGroup sequenceGroup)
        {
            this.SessionId = sessionId;
            this._contextInst = new DesignTimeContext(sequenceGroup);
        }

        #region 初始化
        public void Initialize()
        {
            
        }

        public void Dispose()
        {

        }
        #endregion

        #region SequenceGroup Argument 没有实现
        public IArgument AddArgument(string name, ITypeData type)
        {
            throw new NotImplementedException();
        }
        public IArgument RemoveArgument(string name)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 设计时支持 先不实现
        public IVariableCollection GetFittedVariables(ITypeData type)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 指数counter
        public ILoopCounter AddLoopCounter(ISequenceStep sequenceStep, int maxCount)
        {
            ILoopCounter loopCounter = _sequenceManager.CreateLoopCounter();
            loopCounter.MaxValue = maxCount;
            sequenceStep.LoopCounter = loopCounter;
            ContextSequenceGroupModify();
            return loopCounter;
        }

        public IRetryCounter AddRetryCounter(ISequenceStep sequenceStep, int maxCount)
        {
            IRetryCounter retryCounter = _sequenceManager.CreateRetryCounter();
            retryCounter.MaxRetryTimes = maxCount;
            sequenceStep.RetryCounter = retryCounter;
            ContextSequenceGroupModify();
            return retryCounter;
        }
 
        public ILoopCounter RemoveLoopCounter(ISequenceStep sequenceStep)
        {
            ILoopCounter loopCounter = sequenceStep.LoopCounter;
            sequenceStep.LoopCounter = null;
            ContextSequenceGroupModify();
            return loopCounter;
        }

        public IRetryCounter RemoveRetryCounter(ISequenceStep sequenceStep)
        {
            IRetryCounter retryCounter = sequenceStep.RetryCounter;
            sequenceStep.RetryCounter = null;
            ContextSequenceGroupModify();
            return retryCounter;
        }

        public void ModifyCounter(ILoopCounter counter, int maxCount, bool enabled)
        {
            counter.MaxValue = maxCount;
            counter.CounterEnabled = enabled;
            ContextSequenceGroupModify();
        }

        public void ModifyCounter(IRetryCounter counter, int maxCount, bool enabled)
        {
            counter.MaxRetryTimes = maxCount;
            counter.RetryEnabled = enabled;
            ContextSequenceGroupModify();
        }
        #endregion

        #region Sequence
        public ISequence AddSequence(ISequence sequence, int index)
        {
            if(index == -1)
            {
                sequence.Index = -1;
                Context.SequenceGroup.SetUp = sequence;
            }
            else if(index == -2)
            {
                sequence.Index = -2;
                Context.SequenceGroup.TearDown = sequence;
            }
            else
            {
                Context.SequenceGroup.Sequences.Insert(index, sequence);
            }
            sequence.Parent = Context.SequenceGroup;
            //sequenceGroup有变动
            ContextSequenceGroupModify();
            return sequence;
        }
        public ISequence AddSequence(string sequenceName, string description, int index)
        {
            ISequence sequence = _sequenceManager.CreateSequence();
            sequence.Name = sequenceName;
            sequence.Description = description;
            return AddSequence(sequence, index);
        }

        //Remove Sequence先判断是否为Setup, Teardown（是，则创建空白sequences填入）
        //如果不是再循环sequences移除
        //两个remove返回布尔，一个不返回，对应封装里的Remove跟RemoveAt方法所返回的值
        public bool RemoveSequence(string sequenceName, string description)
        {
            //找sequence
            if (sequenceName.Equals(Context.SequenceGroup.SetUp.Name))
            {
                return RemoveSequence(Context.SequenceGroup.SetUp);
            }
            else if (sequenceName.Equals(Context.SequenceGroup.TearDown.Name))
            {
                return RemoveSequence(Context.SequenceGroup.TearDown);
            }
            else
            {
                ISequence sequence = Context.SequenceGroup.Sequences.FirstOrDefault(item => item.Name.Equals(sequenceName) && item.Description.Equals(description));
                return RemoveSequence(sequence);
            }
        }

        public void RemoveSequence(int index)
        {
            if(index == -1)
            {
                Context.SequenceGroup.SetUp = _sequenceManager.CreateSequence();
            }
            else if(index == -2)
            {
                Context.SequenceGroup.TearDown = _sequenceManager.CreateSequence();
            }
            else
            {
                Context.SequenceGroup.Sequences.RemoveAt(index);
            }
            ContextSequenceGroupModify();
        }

        public bool RemoveSequence(ISequence sequence)
        {
            if (Context.SequenceGroup.SetUp.Equals(sequence))
            {
                Context.SequenceGroup.SetUp = _sequenceManager.CreateSequence();
                return true;
            }
            else if (Context.SequenceGroup.TearDown.Equals(sequence))
            {
                Context.SequenceGroup.TearDown = _sequenceManager.CreateSequence();
                return true;
            }
            else
            {
                bool removed = Context.SequenceGroup.Sequences.Remove(sequence);
                if (removed)
                {
                    ContextSequenceGroupModify();
                }
                return removed;
            }
        }

        public ISequence GetSequence(int sequenceId)
        {
            if (sequenceId == -1)
            {
                return Context.SequenceGroup.SetUp;
            }
            else if (sequenceId == -2)
            {
                return Context.SequenceGroup.TearDown;
            }
            else
            {
                return Context.SequenceGroup.Sequences[sequenceId];
            }
        }

        //to do I18n
        public ISequence GetSequence(string name)
        {
            if (name.Equals(Context.SequenceGroup.SetUp.Name))
            {
                return Context.SequenceGroup.SetUp;
            }
            else if (name.Equals(Context.SequenceGroup.TearDown.Name))
            {
                return Context.SequenceGroup.TearDown;
            }
            else
            {
                ISequence sequence = Context.SequenceGroup.Sequences.FirstOrDefault(item => item.Name.Equals(name));
                if(sequence == null)
                {
                    throw new TestflowDataException(ModuleErrorCode.TargetNotExist, $"Sequence {name} not found in session");
                }
                return sequence;
            }
        }
        #endregion

        #region Step
        public ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, ISequenceStep stepData, int index)
        {
            if (parent is ISequence)
            {
                ((ISequence)parent).Steps.Insert(index, stepData);
            }
            else if (parent is ISequenceStep)
            {
                ISequenceStep parentStep = (ISequenceStep)parent;
                if (parentStep.StepType == SequenceStepType.TryFinallyBlock ||
                    parentStep.StepType == SequenceStepType.SequenceCall)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, 
                        i18N.GetStr("InvalidOperation"));
                }
                parentStep.SubSteps.Insert(index, stepData);
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent needs to be Sequence or SequenceStep");
            }
            stepData.Parent = parent;
            ContextSequenceGroupModify();
            return stepData;
        }

        public ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, IList<ISequenceStep> stepDatas, int index)
        {
            if (parent is ISequence)
            {
                for (int n=0;n < stepDatas.Count; n++)
                {
                    ((ISequence)parent).Steps.Insert(n + index, stepDatas[n]);
                    stepDatas[n].Parent = parent;
                }
            }
            else if (parent is ISequenceStep)
            {
                for (int n = 0; n < stepDatas.Count; n++)
                {
                    ISequenceStep parentStep = (ISequenceStep)parent;
                    if (parentStep.StepType == SequenceStepType.TryFinallyBlock ||
                        parentStep.StepType == SequenceStepType.SequenceCall)
                    {
                        I18N i18N = I18N.GetInstance(Constants.I18nName);
                        throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation,
                            i18N.GetStr("InvalidOperation"));
                    }
                    parentStep.SubSteps.Insert(n + index, stepDatas[n]);
                    stepDatas[n].Parent = parent;
                }
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent needs to be Sequence or SequenceStep");
            }


            ContextSequenceGroupModify();
            return stepDatas[0];
        }

        public ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, string name, string description, int index)
        {
            ISequenceStep sequenceStep = _sequenceManager.CreateSequenceStep(true);
            sequenceStep.Name = name;
            sequenceStep.Description = description;
            return AddSequenceStep(parent, sequenceStep, index);  
        }

        public ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, IFunctionData functionData, string name, string description, int index)
        {
            ISequenceStep sequenceStep = _sequenceManager.CreateSequenceStep();
            sequenceStep.Function = functionData;
            sequenceStep.Name = name;
            sequenceStep.Description = description;
            return AddSequenceStep(parent, sequenceStep, index);
        }

        //todo I18n
        public void RemoveSequenceStep(ISequenceFlowContainer parent, string name)
        {
            ISequenceStep step = null;
            if (parent is ISequence)
            {
                step = ((ISequence)parent).Steps.FirstOrDefault(item => item.Name.Equals(name));
            }
            else if (parent is ISequenceStep)
            {
                step = ((ISequenceStep)parent).SubSteps.FirstOrDefault(item => item.Name.Equals(name));
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent needs to be Sequence or SequenceStep");
            }
            if (step == null)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, $"Step {name} could not be found in parent {parent.Name}");
            }
            RemoveSequenceStep(parent, step);
        }

        //todo I18n
        public void RemoveSequenceStep(ISequenceFlowContainer parent, int index)
        {
            if (parent is ISequence)
            {
                ((ISequence)parent).Steps.RemoveAt(index);
            }
            else if (parent is ISequenceStep)
            {
                ISequenceStep parentStep = (ISequenceStep)parent;
                if (parentStep.StepType == SequenceStepType.TryFinallyBlock ||
                    parentStep.StepType == SequenceStepType.SequenceCall)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation,
                        i18N.GetStr("InvalidOperation"));
                }
                (parentStep).SubSteps.RemoveAt(index);
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent needs to be Sequence or SequenceStep");
            }
            ContextSequenceGroupModify();
        }

        //todo I18n
        public void RemoveSequenceStep(ISequenceFlowContainer parent, ISequenceStep step)
        {
            bool removed = false;
            if (parent is ISequence)
            {
                removed = ((ISequence)parent).Steps.Remove(step);
            }
            else if (parent is ISequenceStep)
            {
                ISequenceStep parentStep = (ISequenceStep)parent;
                if (parentStep.StepType == SequenceStepType.TryFinallyBlock ||
                    parentStep.StepType == SequenceStepType.SequenceCall)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation,
                        i18N.GetStr("InvalidOperation"));
                }
                removed = (parentStep).SubSteps.Remove(step);
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent needs to be Sequence or SequenceStep");
            }

            //判断成功remove与否
            if (removed)
            {
                ContextSequenceGroupModify();
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, $"Step {step.Name} could not be found in parent {parent.Name}");
            }
        }

        public ISequenceStep GetSequenceStep(int sequenceIndex, params int[] stepIndex)
        {
            ISequenceStepCollection stepCollection = Context.SequenceGroup.Sequences[sequenceIndex].Steps;
            ISequenceStep step = stepCollection[stepIndex[0]];
            for (int n=1;n < stepIndex.Length;n++)
            {
                //todo:need to check hasSubSteps()
                //need to check stepIndex valid
                step = step.SubSteps[stepIndex[n]];
            }
            return step;
        }
        #endregion

        #region 加减变量, 设置变量, parent是Isequence或Isequencegroup或testProject todo判定index正确性

        public IVariable FindVariable(string variableName, ISequence sequence)
        {
            IVariable variable = FindVariableInParent(variableName, sequence);
            if(variable == null)
            {
                variable = FindVariableInParent(variableName, Context.SequenceGroup);
            }
            return variable;
        }

        //todo I18n
        public IVariable FindVariableInParent(string variableName, ISequenceFlowContainer parent)
        {
            IVariableCollection variables;
            if(parent is ISequenceGroup)
            {
                variables = ((ISequenceGroup)parent).Variables;
            }
            else if(parent is ISequence)
            {
                variables = ((ISequence)parent).Variables;
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent must be ISequenceGroup or ISequence.");
            }

            return variables.FirstOrDefault(item => item.Name.Equals(variableName));
        }

        public IVariable AddVariable(ISequenceFlowContainer parent, string variableName, string value, int index)
        {
            IVariable variable = _sequenceManager.CreateVarialbe();
            variable.Name = variableName;
            variable.Value = value;
            return AddVariable(parent, variable, index);
        }

        //todo I18n
        public IVariable AddVariable(ISequenceFlowContainer parent, IVariable variable, int index)
        {
            if(FindVariableInParent(variable.Name, parent) != null)
            {
                throw new TestflowDataException(ModuleErrorCode.DuplicateDefinition, $"Variable with name {variable.Name} exists in {parent.Name}.");
            }

            if (parent is ISequenceGroup)
            {
                ((ISequenceGroup)parent).Variables.Insert(index, variable);
            }
            else if (parent is ISequence)
            {
                ((ISequence)parent).Variables.Insert(index, variable);
            }
            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidEditOperation, "Parent must be ISequenceGroup or ISequence.");
            }

            variable.Parent = parent;
            return variable;
        }

        //todo I18n
        public IVariable RemoveVariable(ISequenceFlowContainer parent, IVariable variable)
        {
            bool removed = false;
            if (parent is ISequenceGroup)
            {
                removed = ((ISequenceGroup)parent).Variables.Remove(variable);
            }
            if (parent is ISequence)
            {
                removed = ((ISequence)parent).Variables.Remove(variable);
            }

            if (!removed)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, $"Variable not found in parent {parent.Name}");
            }
            return variable;
        }

        //todo I18n
        public IVariable RemoveVariable(ISequenceFlowContainer parent, string variableName)
        {
            IVariable variable = null;
            bool removed = false;
            if (parent is ISequenceGroup)
            {
                variable = ((ISequenceGroup)parent).Variables.FirstOrDefault(item => item.Name.Equals(variableName));
                removed = ((ISequenceGroup)parent).Variables.Remove(variable);
            }
            if (parent is ISequence)
            {
                variable = ((ISequence)parent).Variables.FirstOrDefault(item => item.Name.Equals(variableName));
                removed = ((ISequence)parent).Variables.Remove(variable);
            }

            if (!removed)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, $"Variable not found in parent {parent.Name}");
            }
            return variable;
            
        }

        public void SetVariableValue(IVariable variable, string value)
        {
            variable.Value = value;
        }

        //todo I18n
        public void SetVariableValue(string variableName, string value)
        {
            IVariable variable = ModuleUtils.FindVariableInSequenceGroup(variableName, Context.SequenceGroup);
            if (variable == null)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, "Variable not found in current session");
            }
            variable.Value = value;
        }

        public void SetVariableType(IVariable variable, ITypeData typeData)
        {
            variable.Type = typeData;
            if (TypeChecker.CheckForValueType(typeData))
            {
                variable.VariableType = VariableType.Value;
            }
        }

        ////todo I18n
        public void SetVariableType(string variableName, ITypeData typeData)
        {
            IVariable variable = ModuleUtils.FindVariableInSequenceGroup(variableName, Context.SequenceGroup);
            if (variable == null)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, "Variable not found in current session");
            }
            SetVariableType(variable, typeData);
        }
        #endregion

        #region 设置Instance, Return, Parameters
        //todo sequenceindex, substep index报错; 还有createFunctionData(null)
        public void SetInstance(string variableName, int sequenceIndex, params int[] indexes)
        {
            //todo
            ISequence sequence = Context.SequenceGroup.Sequences[sequenceIndex];
            ISequenceStep step = sequence.Steps[indexes[0]];
            if (indexes.Length > 1)
            {
                int n = 1;
                while (n < indexes.Length)
                {
                    step = step.SubSteps[indexes[n]];
                    n++;
                }
            }
            SetInstance(variableName, step);
        }

        public void SetInstance(string variableName, ISequenceStep sequence)
        {
            sequence.Function.Instance = variableName;
            ContextSequenceGroupModify();
        }

        public void SetParameterValue(string parameterName, string value, ParameterType parameterType, int sequenceIndex, params int[] indexes)
        {
            ISequence sequence = Context.SequenceGroup.Sequences[sequenceIndex];
            ISequenceStep step = sequence.Steps[indexes[0]];
            if (indexes.Length > 1)
            {
                int n = 1;
                while (n < indexes.Length)
                {
                    step = step.SubSteps[indexes[n]];
                    n++;
                }
            }
            SetParameterValue(parameterName, value, parameterType, step);
        }

        public void SetParameterValue(string parameterName, string value, ParameterType parameterType,ISequenceStep sequence)
        {
            IParameterData data = ModuleUtils.FindParameterByName(parameterName, sequence);
            data.Value = value;
            data.ParameterType = parameterType;
        }

        //todo sequenceindex, substep index报错; 还有createFunctionData(null)
        public void SetReturn(string variableName, int sequenceIndex, params int[] indexes)
        {
            //todo
            ISequence sequence = Context.SequenceGroup.Sequences[sequenceIndex];
            ISequenceStep step = sequence.Steps[indexes[0]];
            if (indexes.Length > 1)
            {
                int n = 1;
                while (n < indexes.Length)
                {
                    step = step.SubSteps[indexes[n]];
                    n++;
                }
            }
            SetReturn(variableName, step);
        }

        public void SetReturn(string variableName, ISequenceStep sequence)
        {
            sequence.Function.Return = variableName;
            ContextSequenceGroupModify();
        }
        #endregion
    }
}
