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
using Testflow.SequenceManager.SequenceElements;
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
                ? String.Format(relativePathFormat, @"\\")
                : String.Format(relativePathFormat, dirDelim);
            _absolutePathRegex = new Regex(relativePathRegexStr);

            _availableDirs = new List<string>(workspaceDirs.Length + 2);
            _availableDirs.AddRange(workspaceDirs); 
            _availableDirs.Add(platformLibDir);
            _availableDirs.Add(dotNetLibDir);
            _availableDirs.Add(dotNetRootDir);

            _pathCache = new StringBuilder(200);
        }

        #region 配置所有路径为相对路径

        public void SetAssembliesToRelativePath(ITestProject testProject, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            // 将当前序列路径作为首要判断路径
            _availableDirs.Insert(0, seqDir);
            SetAssembliesToRelativePath(testProject.Assemblies);
            _availableDirs.RemoveAt(0);

            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                seqDir = ModuleUtils.GetParentDirectory(sequenceGroup.Info.SequenceGroupFile);

                _availableDirs.Insert(0, seqDir);
                SetAssembliesToRelativePath(sequenceGroup.Assemblies);
                _availableDirs.RemoveAt(0);
            }
        }

        public void SetAssembliesToRelativePath(ISequenceGroup sequenceGroup, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            // 将当前序列路径作为首要判断路径
            _availableDirs.Insert(0, seqDir);
            SetAssembliesToRelativePath(sequenceGroup.Assemblies);
            _availableDirs.RemoveAt(0);
        }

        public void InitSequenceGroupLocations(TestProject testProject, string testProjectPath)
        {
            testProject.SequenceGroupLocations.Clear();
            ISequenceGroupCollection sequenceGroups = testProject.SequenceGroups;
            for (int i = 0; i < sequenceGroups.Count; i++)
            {
                ISequenceGroup sequenceGroup = sequenceGroups[i];
                if (string.IsNullOrWhiteSpace(sequenceGroup.Info.SequenceGroupFile) ||
                    string.IsNullOrWhiteSpace(sequenceGroup.Info.SequenceParamFile))
                {
                    InitSequenceGroupFilePath(sequenceGroup);
                }
                string sequenceGroupPath = ModuleUtils.GetAbsolutePath(sequenceGroup.Info.SequenceGroupFile,
                    testProjectPath);
                string parameterPath = ModuleUtils.GetAbsolutePath(sequenceGroup.Info.SequenceParamFile,
                    testProjectPath);
                if (!ModuleUtils.IsValidFilePath(sequenceGroupPath))
                {
                    sequenceGroupPath = ModuleUtils.GetSequenceGroupPath(testProjectPath, i);
                    parameterPath = ModuleUtils.GetParameterFilePath(sequenceGroupPath);
                    sequenceGroup.Info.SequenceGroupFile = ModuleUtils.GetRelativePath(sequenceGroupPath, testProjectPath);
                    sequenceGroup.Info.SequenceParamFile = ModuleUtils.GetRelativePath(parameterPath, sequenceGroupPath);
                }
                else if (!ModuleUtils.IsValidFilePath(sequenceGroup.Info.SequenceParamFile))
                {
                    parameterPath = ModuleUtils.GetParameterFilePath(sequenceGroupPath);
                    sequenceGroup.Info.SequenceParamFile = ModuleUtils.GetRelativePath(parameterPath, sequenceGroupPath);
                }
                SequenceGroupLocationInfo locationInfo = new SequenceGroupLocationInfo()
                {
                    Name = sequenceGroup.Name,
                    SequenceFilePath = sequenceGroup.Info.SequenceGroupFile,
                    ParameterFilePath = sequenceGroup.Info.SequenceParamFile
                };
                testProject.SequenceGroupLocations.Add(locationInfo);
            }
        }

        // 配置SequenceGroupInfo中序列文件和参数文件的路径为相对路径
        public void SetInfoPathToRelative(ISequenceGroupInfo sequenceGroupInfo)
        {
            string sequenceAbsolutePath = sequenceGroupInfo.SequenceGroupFile;
            sequenceGroupInfo.SequenceGroupFile = ModuleUtils.GetFileName(sequenceAbsolutePath);
            sequenceGroupInfo.SequenceParamFile = ModuleUtils.GetRelativePath(sequenceGroupInfo.SequenceParamFile,
                sequenceAbsolutePath);
        }

        // 配置TestProject中SequenceLocations的位置
        public void SetLocationPath(ISequenceGroupInfo sequenceGroupInfo, SequenceGroupLocationInfo sequenceLocation, string testProjectPath)
        {
            sequenceLocation.SequenceFilePath = ModuleUtils.GetRelativePath(sequenceGroupInfo.SequenceGroupFile,
                testProjectPath);
            sequenceLocation.ParameterFilePath = ModuleUtils.GetRelativePath(sequenceGroupInfo.SequenceParamFile,
                testProjectPath);
        }

        // 配置TestProject中SequenceGroupInfo的序列文件和参数文件的路径为绝对路径
        public void SetInfoPathToAbsolute(ISequenceGroupInfo sequenceGroupInfo, SequenceGroupLocationInfo sequenceLocation, string testProjectPath)
        {
            sequenceGroupInfo.SequenceGroupFile = ModuleUtils.GetAbsolutePath(sequenceLocation.SequenceFilePath, testProjectPath);
            sequenceGroupInfo.SequenceParamFile = ModuleUtils.GetAbsolutePath(sequenceLocation.ParameterFilePath, testProjectPath);
        }

        // 配置SequenceGroupInfo中序列文件和参数文件的路径为绝对路径
        public void SetInfoPathToAbsolute(ISequenceGroupInfo sequenceGroupInfo, string sequenceFullPath)
        {
            sequenceGroupInfo.SequenceGroupFile = ModuleUtils.GetAbsolutePath(sequenceGroupInfo.SequenceGroupFile, sequenceFullPath);
            sequenceGroupInfo.SequenceParamFile = ModuleUtils.GetAbsolutePath(sequenceGroupInfo.SequenceGroupFile, sequenceFullPath);
        }

        // 初始化TestProject中未配置路径的SequenceGroup的路径
        private void InitSequenceGroupFilePath(ISequenceGroup sequenceGroup, string testProjectPath)
        {
            string parentDir = ModuleUtils.GetParentDirectory(testProjectPath);
            sequenceGroup.Info.SequenceGroupFile =
                $"{parentDir}{sequenceGroup.Name}{Path.DirectorySeparatorChar}{sequenceGroup.Name}.{CommonConst.SequenceFileExtension}";
            sequenceGroup.Info.SequenceParamFile =
                $"{parentDir}{sequenceGroup.Name}{Path.DirectorySeparatorChar}{sequenceGroup.Name}.{CommonConst.SequenceDataFileExtension}";
        }

        public void UpdateSequenceGroupInfo(string filePath, ISequenceGroupInfo sequenceGroupInfo)
        {
            string fileRelativePath = ModuleUtils.GetFileName(filePath);
            sequenceGroupInfo.SequenceGroupFile = fileRelativePath;
            sequenceGroupInfo.SequenceParamFile = ModuleUtils.GetParameterFilePath(fileRelativePath);
        }

        private void SetAssembliesToRelativePath(IAssemblyInfoCollection assemblies)
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

        public void SetAssembliesToAbsolutePath(ITestProject testProject, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            _availableDirs.Insert(0, seqDir);
            SetAssembliesToAbsolutePath(testProject.Assemblies);
            _availableDirs.RemoveAt(0);

            SequenceGroupLocationInfoCollection seqGroupLocations = ((TestProject)testProject).SequenceGroupLocations;
            ISequenceGroupCollection sequenceGroups = testProject.SequenceGroups;
            for (int i = 0; i < sequenceGroups.Count; i++)
            {
                string seqGroupDir = ModuleUtils.GetParentDirectory(seqDir + seqGroupLocations[i].SequenceFilePath);

                _availableDirs.Insert(0, seqGroupDir);
                SetAssembliesToAbsolutePath(sequenceGroups[i].Assemblies);
                _availableDirs.RemoveAt(0);
            }
        }

        public void SetAssembliesToAbsolutePath(ISequenceGroup sequenceGroup, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            _availableDirs.Insert(0, seqDir);
            SetAssembliesToAbsolutePath(sequenceGroup.Assemblies);
            _availableDirs.RemoveAt(0);
        }

        private void SetAssembliesToAbsolutePath(IAssemblyInfoCollection assemblies)
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