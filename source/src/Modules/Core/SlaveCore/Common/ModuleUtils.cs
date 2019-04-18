using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Runner.Model;

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

        public static StepModelBase CreateStepModelChain(IList<ISequenceStep> steps, SlaveContext context)
        {
            StepModelBase root = null;
            if (steps.Count == 0)
            {
                return root;
            }

            root = StepModelBase.GetStepModel(steps[0], context);
            root.NextStep = null;
            StepModelBase lastNode = root;
            StepModelBase currentNode = null;
            for (int i = 1; i < steps.Count; i++)
            {
                currentNode = StepModelBase.GetStepModel(steps[i], context);
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
    }
}