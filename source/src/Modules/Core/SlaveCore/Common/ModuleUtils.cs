using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Testflow.Common;
using Testflow.CoreCommon;
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

        public static StepTaskEntityBase CreateStepModelChain(IList<ISequenceStep> steps, SlaveContext context)
        {
            StepTaskEntityBase root = null;
            if (steps.Count == 0)
            {
                return root;
            }

            root = StepTaskEntityBase.GetStepModel(steps[0], context);
            root.NextStep = null;
            StepTaskEntityBase lastNode = root;
            StepTaskEntityBase currentNode = null;
            for (int i = 1; i < steps.Count; i++)
            {
                currentNode = StepTaskEntityBase.GetStepModel(steps[i], context);
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
                variable = ((ITestProject) sequenceData).Variables.FirstOrDefault(item => item.Name == rawVarName);
            }
            else if (sequenceData is ISequenceGroup)
            {
                variable = ((ISequenceGroup) sequenceData).Variables.FirstOrDefault(item => item.Name == rawVarName);
            }
            else if (sequenceData is ISequenceStep)
            {
                variable = ((ISequence)sequenceData).Variables.FirstOrDefault(item => item.Name == rawVarName);
                if (null == variable)
                {
                    variable =
                        ((ISequenceGroup) sequenceData.Parent).Variables.FirstOrDefault(item => item.Name == rawVarName);
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

        public static object GetParamValue(string paramValueStr, object varValue)
        {
            if (!paramValueStr.Contains(Constants.PropertyDelim))
            {
                return varValue;
            }
            object paramValue = varValue;
            string[] paramElems = paramValueStr.Split(Constants.PropertyDelim.ToCharArray());
            BindingFlags binding = BindingFlags.Public | BindingFlags.Instance;
            for (int i = 1; i < paramElems.Length; i++)
            {
                PropertyInfo propertyInfo = paramValue.GetType().GetProperty(paramElems[i], binding);
                if (null == propertyInfo)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetFStr("UnexistVariable", paramValueStr));
                }
                paramValue = propertyInfo.GetValue(paramValue);
            }
            return paramValue;
        }

        public static object SetParamValue(string paramValueStr, object varValue, object paramValue)
        {
            if (!paramValueStr.Contains(Constants.PropertyDelim))
            {
                return varValue;
            }
            object parentValue = varValue;
            Type parentType = varValue.GetType();
            string[] paramElems = paramValueStr.Split(Constants.PropertyDelim.ToCharArray());
            BindingFlags binding = BindingFlags.Public | BindingFlags.Instance;
            for (int i = 1; i < paramElems.Length - 1; i++)
            {
                PropertyInfo propertyInfo = parentType.GetProperty(paramElems[i], binding);
                if (null == propertyInfo)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetFStr("UnexistVariable", paramValueStr));
                }
                parentType = propertyInfo.PropertyType;
                parentValue = propertyInfo.GetValue(parentValue);
            }
            PropertyInfo paramProperty = parentType.GetProperty(paramElems[paramElems.Length - 1], binding);
            if (null == paramProperty)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetFStr("UnexistVariable", paramValueStr));
            }
            paramProperty.SetValue(parentValue, paramValue);
            return varValue;
        }

        // TODO 暂时写死，使用AppDomain为单位计算
        public static void FillPerformance(StatusMessage message)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            message.Performance.ProcessorTime = currentDomain.MonitoringTotalProcessorTime.TotalMilliseconds;
            message.Performance.MemoryUsed = currentDomain.MonitoringSurvivedMemorySize;
            message.Performance.MemoryAllocated = currentDomain.MonitoringTotalAllocatedMemorySize;
        }

        public static CallStack GetSequenceStack(int index)
        {
            return StepTaskEntityBase.GetCurrentStep(index).GetStack();
        }
    }
}