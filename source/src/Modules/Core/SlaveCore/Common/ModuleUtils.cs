using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Runner.Model;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore.Common
{
    internal static class ModuleUtils
    {
        public static bool IsAbosolutePath(string path)
        {
            char dirDelim = Path.DirectorySeparatorChar;
            // 绝对路径匹配模式，如果匹配则path已经是绝对路径
            string regexFormat = dirDelim.Equals('\\')
                ? $"^(([a-zA-z]:)?{dirDelim}{dirDelim})"
                : $"^(([a-zA-z]:)?{dirDelim})";
            Regex regex = new Regex(regexFormat);
            return regex.IsMatch(path);
        }

        public static string GetFileFullPath(string path, string parentPath)
        {
            //            if (IsAbosolutePath(path))
            //            {
            //                return File.Exists(path) ? path : null;
            //            }
            if (string.IsNullOrWhiteSpace(parentPath))
            {
                return File.Exists(path) ? path : null;
            }
            string dirDelim = Path.DirectorySeparatorChar.ToString();
            if (!parentPath.EndsWith(dirDelim))
            {
                parentPath += dirDelim;
            }
            if (path.StartsWith(dirDelim))
            {
                path = path.Remove(path.Length - 1, 1);
            }
            string fullPath = parentPath + path;
            return File.Exists(fullPath) ? fullPath : null;
        }

        public static string GetTypeFullName(ITypeData typeData)
        {
            return $"{typeData.Namespace}.{typeData.Name}";
        }

        public static string GetTypeFullName(Type typeData)
        {
            return $"{typeData.Namespace}.{typeData.Name}";
        }

        public static StepTaskEntityBase CreateStepModelChain(IList<ISequenceStep> steps, SlaveContext context,
            int sequenceIndex)
        {
            StepTaskEntityBase root = null;
            if (steps.Count == 0)
            {
                context.LogSession.Print(LogLevel.Debug, context.SessionId,
                    $"Empty steps created for sequence {sequenceIndex}");
                return StepTaskEntityBase.GetEmptyStepModel(context, sequenceIndex);
            }

            root = StepTaskEntityBase.GetStepModel(steps[0], context, sequenceIndex);
            root.NextStep = null;
            StepTaskEntityBase lastNode = root;
            StepTaskEntityBase currentNode = null;
            for (int i = 1; i < steps.Count; i++)
            {
                currentNode = StepTaskEntityBase.GetStepModel(steps[i], context, sequenceIndex);
                lastNode.NextStep = currentNode;
                lastNode = currentNode;
                currentNode.NextStep = null;
            }
            return root;
        }

        public static StepTaskEntityBase CreateSubStepModelChain(IList<ISequenceStep> steps, SlaveContext context, int sequenceIndex)
        {
            StepTaskEntityBase root = null;
            if (steps.Count == 0)
            {
                return root;
            }

            root = StepTaskEntityBase.GetStepModel(steps[0], context, sequenceIndex);
            root.NextStep = null;
            StepTaskEntityBase lastNode = root;
            StepTaskEntityBase currentNode = null;
            for (int i = 1; i < steps.Count; i++)
            {
                currentNode = StepTaskEntityBase.GetStepModel(steps[i], context, sequenceIndex);
                lastNode.NextStep = currentNode;
                lastNode = currentNode;
                currentNode.NextStep = null;
            }
            return root;
        }

        public static IVariable GetVaraibleByRawVarName(string rawVarName, ISequenceFlowContainer sequenceData)
        {
            IVariable variable = null;
            if (sequenceData is ITestProject)
            {
                variable = ((ITestProject)sequenceData).Variables.FirstOrDefault(item => item.Name == rawVarName);
            }
            else if (sequenceData is ISequenceGroup)
            {
                variable = ((ISequenceGroup)sequenceData).Variables.FirstOrDefault(item => item.Name == rawVarName);
            }
            else if (sequenceData is ISequence)
            {
                variable = ((ISequence)sequenceData).Variables.FirstOrDefault(item => item.Name == rawVarName);
                if (null == variable)
                {
                    variable =
                        ((ISequenceGroup)sequenceData.Parent).Variables.FirstOrDefault(item => item.Name == rawVarName);
                }
            }
            else if (sequenceData is ISequenceStep)
            {
                ISequenceFlowContainer sequence = sequenceData;
                do
                {
                    sequence = sequence.Parent;
                } while (sequence is ISequenceStep);
                variable = ((ISequence)sequence).Variables.FirstOrDefault(item => item.Name == rawVarName);
                if (null == variable)
                {
                    variable =
                        ((ISequenceGroup)sequence.Parent).Variables.FirstOrDefault(item => item.Name == rawVarName);
                }
            }
            return variable;
        }

        public static string GetVariableNameFromParamValue(string paramValue)
        {
            if (!paramValue.Contains(Constants.PropertyDelim))
            {
                return paramValue;
            }
            string[] valueElems = paramValue.Split(Constants.PropertyDelim.ToCharArray());
            return valueElems[0];
        }

        

        // TODO 暂时写死，使用AppDomain为单位计算
        public static void FillPerformance(StatusMessage message)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            double processorTime = currentDomain.MonitoringTotalProcessorTime.TotalMilliseconds;
            long survivedMemorySize = currentDomain.MonitoringSurvivedMemorySize;
            long totalAllocatedMemorySize = currentDomain.MonitoringTotalAllocatedMemorySize;
            if (null == message.Performance)
            {
                message.Performance = new PerformanceData(survivedMemorySize, totalAllocatedMemorySize, processorTime);
            }
            else
            {
                message.Performance.ProcessorTime = processorTime;
                message.Performance.MemoryUsed = survivedMemorySize;
                message.Performance.MemoryAllocated = totalAllocatedMemorySize;
            }
        }

        public static string GetFullParameterVariableName(string fullVariableName, string paramValue)
        {
            if (!paramValue.Contains(Constants.PropertyDelim))
            {
                return fullVariableName;
            }
            string[] elems = paramValue.Split(Constants.PropertyDelim.ToCharArray());
            elems[0] = fullVariableName;
            return string.Join(Constants.PropertyDelim, elems);
        }

        public static CallStack GetSequenceStack(int index)
        {
            return StepTaskEntityBase.GetCurrentStep(index).GetStack();
        }

        public static string GetVariableFullName(string variableName, ISequenceStep step, int session)
        {
            while (step.Parent is ISequenceStep)
            {
                step = (ISequenceStep)step.Parent;
            }
            ISequence sequence = (ISequence)step.Parent;
            IVariable variable = sequence.Variables.FirstOrDefault(item => item.Name.Equals(variableName));

            if (null != variable)
            {
                return CoreUtils.GetRuntimeVariableName(session, variable);
            }
            ISequenceGroup sequenceGroup = (ISequenceGroup)sequence.Parent;
            variable = sequenceGroup.Variables.First(item => item.Name.Equals(variableName));
            return CoreUtils.GetRuntimeVariableName(session, variable);
        }
    }
}