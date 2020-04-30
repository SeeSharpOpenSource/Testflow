using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager
{
    internal class DirectoryHelper
    {
        private readonly List<string> _availableDirs;
        private readonly StringBuilder _pathCache;
        private readonly Regex _absolutePathRegex;

        public DirectoryHelper(IModuleConfigData configData)
        {
            string dotNetLibDir = configData.GetProperty<string>("DotNetLibDir");
            string dotNetRootDir = configData.GetProperty<string>("DotNetRootDir");
            string platformLibDir = configData.GetProperty<string>("PlatformLibDir");
            string[] workspaceDirs = configData.GetProperty<string[]>("WorkspaceDir");

            // 本来用上面就可以确定，因为前期版本里对相对路径前加了\，导致相对路径在第一条下会被判定为绝对路径，所以在此做当前处理
            // 暂未考虑Linux的问题，后续版本会去除这里的判断
//            string relativePathFormat = @"^([a-zA-Z]:)?{0}";
            string relativePathFormat = @"^[a-zA-Z]:{0}";
            char dirDelim = Path.DirectorySeparatorChar;
            // \在正则表达式中需要转义
            string relativePathRegexStr = dirDelim.Equals('\\')
                ? string.Format(relativePathFormat, @"\\")
                : string.Format(relativePathFormat, dirDelim);
            _absolutePathRegex = new Regex(relativePathRegexStr);

            _availableDirs = new List<string>(workspaceDirs.Length + 2);
            _availableDirs.AddRange(workspaceDirs);
            _availableDirs.Add(platformLibDir);
            _availableDirs.Add(dotNetLibDir);
            _availableDirs.Add(dotNetRootDir);

            _pathCache = new StringBuilder(200);
        }

        #region 配置所有路径为相对路径

        public void SetToRelativePath(ITestProject testProject, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            // 将当前序列路径作为首要判断路径
            _availableDirs.Insert(0, seqDir);

            SetToRelativePath(testProject.Assemblies);
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                SetToRelativePath(sequenceGroup.Assemblies);
            }

            _availableDirs.RemoveAt(0);
        }

        public void SetToRelativePath(ISequenceGroup sequenceGroup, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            // 将当前序列路径作为首要判断路径
            _availableDirs.Insert(0, seqDir);

            SetToRelativePath(sequenceGroup.Assemblies);

            _availableDirs.RemoveAt(0);
        }

        private void SetToRelativePath(IAssemblyInfoCollection assemblies)
        {
            foreach (IAssemblyInfo assemblyInfo in assemblies)
            {
                if (IsRelativePath(assemblyInfo.Path))
                {
                    continue;
                }
                // 如果包含在可用路径内则替换为相对路径
                foreach (string availableDir in _availableDirs)
                {
                    if (assemblyInfo.Path.StartsWith(availableDir, StringComparison.OrdinalIgnoreCase))
                    {
                        // 将绝对路径截取为相对路径
                        assemblyInfo.Path = assemblyInfo.Path.Substring(availableDir.Length,
                            assemblyInfo.Path.Length - availableDir.Length);
                        break;
                    }
                }
            }
        }

        #endregion

        #region 配置所有路径为绝对路径

        public void SetToAbsolutePath(ITestProject testProject, string filePath)
        {
            SetToAbsolutePath(testProject.Assemblies);
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                SetToAbsolutePath(sequenceGroup.Assemblies);
            }
        }

        public void SetToAbsolutePath(ISequenceGroup sequenceGroup, string filePath)
        {
            SetToAbsolutePath(sequenceGroup.Assemblies);
        }

        private void SetToAbsolutePath(IAssemblyInfoCollection assemblies)
        {
            foreach (IAssemblyInfo assemblyInfo in assemblies)
            {
                // 如果是绝对路径则继续执行
                string path = assemblyInfo.Path;
                if (!IsRelativePath(path))
                {
                    continue;
                }
                string abosolutePath = InternalGetAbosolutePath(path);
                //　如果没找到库，则抛出异常
                if (null == abosolutePath)
                {
                    ILogService logService = TestflowRunner.GetInstance().LogService;
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"Assembly '{assemblyInfo.AssemblyName}' cannot be found in '{assemblyInfo.Path}'.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.DeSerializeFailed,
                        i18N.GetFStr("AssemblyCannotFound", assemblyInfo.AssemblyName));
                }
                assemblyInfo.Path = abosolutePath;
            }
        }

        private string InternalGetAbosolutePath(string path)
        {
            string abosolutePath = null;
            // 如果文件名前面有分隔符则去掉
            while (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Substring(1, path.Length - 1);
            }
            // 转换为绝对路径时反向寻找，先.net库再平台库再用户库
            for (int i = _availableDirs.Count - 1; i >= 0; i--)
            {
                _pathCache.Clear();
                string availableDir = _availableDirs[i];
                _pathCache.Append(availableDir).Append(path);
                // 如果库存在则配置为绝对路径，然后返回
                if (File.Exists(_pathCache.ToString()))
                {
                    abosolutePath = _pathCache.ToString();
                    break;
                }
            }
            return abosolutePath;
        }

        #endregion


        public string GetRelativePath(string path)
        {
            if (IsRelativePath(path))
            {
                return path;
            }
            // 如果包含在可用路径内则不处理，否则替换为相对路径
            foreach (string availableDir in _availableDirs)
            {
                if (!path.StartsWith(availableDir))
                {
                    continue;
                }
                // 将绝对路径截取为相对路径
                return availableDir.Substring(availableDir.Length, path.Length - availableDir.Length);
            }
            return path;
        }

        public string GetAbsolutePath(string path)
        {
            if (!IsRelativePath(path))
            {
                return path;
            }
            string abosolutePath = InternalGetAbosolutePath(path);
            if (null == abosolutePath)
            {
                ILogService logService = TestflowRunner.GetInstance().LogService;
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                    $"File in relative path '{path}' cannot be found.");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.DeSerializeFailed,
                    i18N.GetFStr("FileCannotFound", path));
            }
            return abosolutePath;
        }

        private bool IsRelativePath(string path)
        {
            return !_absolutePathRegex.IsMatch(path);
        }
    }
}