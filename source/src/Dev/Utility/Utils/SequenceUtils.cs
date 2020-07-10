using System.Collections.Generic;
using System.Linq;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.Utility.Utils
{
    /// <summary>
    /// 序列操作相关的工具函数
    /// </summary>
    public static class SequenceUtils
    {
        const string StackDelims = "_";

        static SequenceUtils()
        {
            // 初始化i18n模块
            I18NOption i18NOption = new I18NOption(typeof (Messenger).Assembly, "i18n_utils_zh", "i18n_utils_en")
            {
                Name = UtilityConstants.UtilsName
            };
            I18N.InitInstance(i18NOption);
        }

        #region 使用堆栈获取Step

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static ISequenceStep GetStepFromStack(ITestProject testProject, ICallStack callStack)
        {
            if (callStack.StepStack.Count <= 0)
            {
                return null;
            }
            int sessionId = callStack.Session;
            int sequenceIndex = callStack.Sequence;
            ISequence sequence = GetSequence(testProject, sessionId, sequenceIndex);
            return GetStepFromStack(sequence, callStack.StepStack, callStack.ToString());
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static ISequenceStep GetStepFromStack(ISequenceGroup sequenceGroup, ICallStack callStack)
        {
            if (callStack.StepStack.Count <= 0)
            {
                return null;
            }
            int sessionId = callStack.Session;
            int sequenceIndex = callStack.Sequence;
            ISequence sequence = GetSequence(sequenceGroup, sessionId, sequenceIndex);
            return GetStepFromStack(sequence, callStack.StepStack, callStack.ToString());
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static ISequenceStep GetStepFromStack(ITestProject testProject, string stackStr)
        {
            string[] stackElems = stackStr.Split(StackDelims.ToCharArray());
            if (stackElems.Length < 3)
            {
                return null;
            }
            int sessionId = int.Parse(stackElems[0]);
            int sequenceIndex = int.Parse(stackElems[1]);
            List<int> stack = new List<int>(stackElems.Length - 2);
            for (int i = 2; i < stackElems.Length; i++)
            {
                stack.Add(int.Parse(stackElems[i]));
            }
            ISequence sequence = GetSequence(testProject, sessionId, sequenceIndex);
            return GetStepFromStack(sequence, stack, stackStr);
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static ISequenceStep GetStepFromStack(ISequenceGroup sequenceGroup, string stackStr)
        {
            string[] stackElems = stackStr.Split(StackDelims.ToCharArray());
            if (stackElems.Length < 3)
            {
                return null;
            }
            int sessionId = int.Parse(stackElems[0]);
            int sequenceIndex = int.Parse(stackElems[1]);
            List<int> stack = new List<int>(stackElems.Length - 2);
            for (int i = 2; i < stackElems.Length; i++)
            {
                stack.Add(int.Parse(stackElems[i]));
            }
            ISequence sequence = GetSequence(sequenceGroup, sessionId, sequenceIndex);
            return GetStepFromStack(sequence, stack, stackStr);
        }

        private static ISequenceStep GetStepFromStack(ISequence sequence, IList<int> stack, string stackStr)
        {
            if (sequence.Steps.Count <= stack[0])
            {
                // 如果是Stack长度为1，且第一个StepIndex为0，则说明是空序列的占位符，返回null
                if (stack[0] == 0 && stack.Count == 1)
                {
                    return null;
                }
                I18N i18N = I18N.GetInstance(UtilityConstants.UtilsName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                    i18N.GetFStr("InvalidCallStack", stackStr));
            }
            ISequenceStep step = sequence.Steps[stack[0]];
            for (int i = 1; i < stack.Count; i++)
            {
                if (!step.HasSubSteps || step.SubSteps.Count < stack[i])
                {
                    I18N i18N = I18N.GetInstance(UtilityConstants.UtilsName);
                    throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                        i18N.GetFStr("InvalidCallStack", stackStr));
                }
                step = step.SubSteps[stack[i]];
            }
            return step;
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static bool TryGetStepFromStack(ITestProject testProject, ICallStack callStack, out ISequenceStep step)
        {
            if (callStack.StepStack.Count <= 0)
            {
                step = null;
                return false;
            }
            int sessionId = callStack.Session;
            int sequenceIndex = callStack.Sequence;
            ISequence sequence = GetSequence(testProject, sessionId, sequenceIndex);
            return TryGetStepFromStack(sequence, callStack.StepStack, callStack.ToString(), out step);
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static bool TryGetStepFromStack(ISequenceGroup sequenceGroup, ICallStack callStack, out ISequenceStep step)
        {
            if (callStack.StepStack.Count <= 0)
            {
                step = null;
                return false;
            }
            int sessionId = callStack.Session;
            int sequenceIndex = callStack.Sequence;
            ISequence sequence = GetSequence(sequenceGroup, sessionId, sequenceIndex);
            return TryGetStepFromStack(sequence, callStack.StepStack, callStack.ToString(), out step);
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static bool TryGetStepFromStack(ITestProject testProject, string stackStr, out ISequenceStep step)
        {
            string[] stackElems = stackStr.Split(StackDelims.ToCharArray());
            if (stackElems.Length < 3)
            {
                step = null;
                return false;
            }
            int sessionId = int.Parse(stackElems[0]);
            int sequenceIndex = int.Parse(stackElems[1]);
            List<int> stack = new List<int>(stackElems.Length - 2);
            for (int i = 2; i < stackElems.Length; i++)
            {
                stack.Add(int.Parse(stackElems[i]));
            }
            ISequence sequence = GetSequence(testProject, sessionId, sequenceIndex);
            return TryGetStepFromStack(sequence, stack, stackStr, out step);
        }

        /// <summary>
        /// 通过Stack获取Step
        /// </summary>
        public static bool TryGetStepFromStack(ISequenceGroup sequenceGroup, string stackStr, out ISequenceStep step)
        {
            string[] stackElems = stackStr.Split(StackDelims.ToCharArray());
            if (stackElems.Length < 3)
            {
                step = null;
                return false;
            }
            int sessionId = int.Parse(stackElems[0]);
            int sequenceIndex = int.Parse(stackElems[1]);
            List<int> stack = new List<int>(stackElems.Length - 2);
            for (int i = 2; i < stackElems.Length; i++)
            {
                stack.Add(int.Parse(stackElems[i]));
            }
            ISequence sequence = GetSequence(sequenceGroup, sessionId, sequenceIndex);
            return TryGetStepFromStack(sequence, stack, stackStr, out step);
        }

        private static bool TryGetStepFromStack(ISequence sequence, IList<int> stack, string stackStr, out ISequenceStep step)
        {
            if (sequence.Steps.Count <= stack[0])
            {
                I18N i18N = I18N.GetInstance(UtilityConstants.UtilsName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                    i18N.GetFStr("InvalidCallStack", stackStr));
            }
            step = sequence.Steps[stack[0]];
            for (int i = 1; i < stack.Count; i++)
            {
                if (!step.HasSubSteps || step.SubSteps.Count < stack[i])
                {
                    step = null;
                    return false;
                }
                step = step.SubSteps[stack[i]];
            }
            return true;
        }

        #endregion

        #region 查找Sequence

        /// <summary>
        /// 通过索引号获取Sequence
        /// </summary>
        public static ISequence GetSequence(ISequenceFlowContainer sequenceData, int sessionId, int sequence)
        {
            if (sequenceData is ITestProject)
            {
                ITestProject testProject = (ITestProject) sequenceData;
                if (sessionId == CommonConst.TestGroupSession)
                {
                    return sequence == CommonConst.SetupIndex
                        ? testProject.SetUp
                        : testProject.TearDown;
                }
                else
                {
                    if (testProject.SequenceGroups.Count <= sessionId || sessionId < 0)
                    {
                        I18N i18N = I18N.GetInstance(UtilityConstants.UtilsName);
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                            i18N.GetStr("CannotFindSequence"));
                    }
                    return GetSequence(testProject.SequenceGroups[sessionId], sequence);
                }

            }
            else if (sequenceData is ISequenceGroup)
            {
                return GetSequence((ISequenceGroup) sequenceData, sequence);
            }
            else if (sequenceData is ISequence)
            {
                return (ISequence) sequenceData;
            }
            else
            {
                I18N i18N = I18N.GetInstance(UtilityConstants.UtilsName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetStr("CannotFindSequence"));
            }
        }

        private static ISequence GetSequence(ISequenceGroup sequenceGroup, int index)
        {
            ISequence sequence;
            switch (index)
            {
                case CommonConst.SetupIndex:
                    sequence = sequenceGroup.SetUp;
                    break;
                case CommonConst.TeardownIndex:
                    sequence = sequenceGroup.TearDown;
                    break;
                default:
                    if (sequenceGroup.Sequences.Count <= index || index < 0)
                    {
                        I18N i18N = I18N.GetInstance(UtilityConstants.UtilsName);
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                            i18N.GetStr("CannotFindSequence"));
                    }
                    sequence = sequenceGroup.Sequences[index];
                    break;
            }
            return sequence;
        }

        /// <summary>
        /// 获取某个序列步骤所在的序列
        /// </summary>
        /// <param name="step">序列步骤</param>
        public static ISequence GetParentSequence(ISequenceStep step)
        {
            ISequenceFlowContainer parent = step.Parent;
            while (parent is ISequenceStep)
            {
                parent = parent.Parent;
            }
            return (ISequence)parent;
        }

        #endregion

        #region 查找变量

        /// <summary>
        /// 判断某个名字的变量是否存在
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="step">使用变量的序列步骤</param>
        public static bool IsVariableExist(string variableName, ISequenceStep step)
        {
            ISequenceFlowContainer parent = step.Parent;
            while (parent is ISequenceStep)
            {
                parent = parent.Parent;
            }
            ISequence sequence = (ISequence) parent;
            if (null == sequence)
            {
                return false;
            }
            return IsVariableExist(variableName, sequence);
        }

        /// <summary>
        /// 判断某个名字的变量是否存在
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="sequence">使用变量的序列</param>
        public static bool IsVariableExist(string variableName, ISequence sequence)
        {
            // 否则则认为表达式为变量值
            if (sequence.Variables.Any(item => item.Name.Equals(variableName)))
            {
                return true;
            }
            ISequenceFlowContainer parent = sequence.Parent;
            if (null == parent)
            {
                return false;
            }
            IVariableCollection rootVariables = parent is ISequenceGroup
                ? ((ISequenceGroup) parent).Variables
                : ((ITestProject) parent).Variables;
            return rootVariables.Any(item => item.Name.Equals(variableName));
        }

        public static IVariable GetVariable(string variableName, ISequenceStep step)
        {
            ISequenceFlowContainer parent = step.Parent;
            while (parent is ISequenceStep)
            {
                parent = parent.Parent;
            }
            ISequence sequence = (ISequence)parent;
            if (null == sequence)
            {
                return null;
            }
            return GetVariable(variableName, sequence);
        }

        public static IVariable GetVariable(string variableName, ISequence sequence)
        {
            IVariable variable = sequence.Variables.FirstOrDefault(item => item.Name.Equals(variableName));
            if (null != variable)
            {
                return variable;
            }
            ISequenceFlowContainer parent = sequence.Parent;
            if (null == parent)
            {
                return null;
            }
            IVariableCollection rootVariables = parent is ISequenceGroup
                ? ((ISequenceGroup) parent).Variables
                : ((ITestProject) parent).Variables;
            variable = rootVariables.FirstOrDefault(item => item.Name.Equals(variableName));
            return variable;
        }

        #endregion

    }
}