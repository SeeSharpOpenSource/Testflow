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

        // 在AvailableDirs中插入序列路径时的索引号
        private int _seqDirIndex;

        public DirectoryHelper(IModuleConfigData configData)
        {
            string dotNetLibDir = configData.GetProperty<string>("DotNetLibDir");
            string dotNetRootDir = configData.GetProperty<string>("DotNetRootDir");
            string platformLibDir = configData.GetProperty<string>("PlatformLibDir");
            string[] workspaceDirs = configData.GetProperty<string[]>("WorkspaceDir");

            // 本来用上面就可以确定，因为前期版本里对相对路径前加了\，导致相对路径在第一条下会被判定为绝对路径，所以在此做当前处理
            // 暂未考虑Linux的问题，后续版本会去除这里的判断
//            string relativePathFormat = @"^([a-zA-Z]:)?{0}";
            const string relativePathFormat = @"^[a-zA-Z]:{0}";
            char dirDelim = Path.DirectorySeparatorChar;
            // \在正则表达式中需要转义
            string relativePathRegexStr = dirDelim.Equals('\\')
                ? string.Format(relativePathFormat, @"\\")
                : string.Format(relativePathFormat, dirDelim);
            _absolutePathRegex = new Regex(relativePathRegexStr);

            _availableDirs = new List<string>(workspaceDirs.Length + 5);
            _availableDirs.Add(dotNetRootDir);
            _availableDirs.Add(dotNetLibDir);
            // 序列当前路径的优先级仅次于dotNet系统库
            _seqDirIndex = _availableDirs.Count;
            _availableDirs.Add(platformLibDir);
            _availableDirs.AddRange(workspaceDirs);

            _pathCache = new StringBuilder(200);
        }

        #region 配置所有路径为相对路径

        public void SetAssembliesToRelativePath(ITestProject testProject, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            // 将当前序列路径作为首要判断路径
            _availableDirs.Insert(_seqDirIndex, seqDir);
            SetAssembliesToRelativePath(testProject.Assemblies);
            _availableDirs.RemoveAt(_seqDirIndex);

            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                seqDir = ModuleUtils.GetParentDirectory(sequenceGroup.Info.SequenceGroupFile);

                _availableDirs.Insert(_seqDirIndex, seqDir);
                try
                {
                    SetAssembliesToRelativePath(sequenceGroup.Assemblies);
                }
                finally
                {
                    _availableDirs.RemoveAt(_seqDirIndex);
                }
            }
        }

        public void SetAssembliesToRelativePath(ISequenceGroup sequenceGroup, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            // 将当前序列路径作为首要判断路径
            _availableDirs.Insert(_seqDirIndex, seqDir);
            try
            {
                SetAssembliesToRelativePath(sequenceGroup.Assemblies);
            }
            finally
            {
                _availableDirs.RemoveAt(_seqDirIndex);
            }
        }

        public void InitSequenceGroupInfoAndLocations(TestProject testProject, string testProjectPath)
        {
            testProject.SequenceGroupLocations.Clear();
            ISequenceGroupCollection sequenceGroups = testProject.SequenceGroups;
            for (int i = 0; i < sequenceGroups.Count; i++)
            {
                ISequenceGroup sequenceGroup = sequenceGroups[i];
                string sequenceGroupDir;
                
                if (ModuleUtils.IsValidFilePath(sequenceGroup.Info.SequenceGroupFile) &&
                    ModuleUtils.IsValidFilePath(sequenceGroup.Info.SequenceParamFile))
                {
                    sequenceGroupDir = ModuleUtils.GetParentDirectory(sequenceGroup.Info.SequenceGroupFile);
                }
                else
                {
                    // 初始化TestProject中未配置路径的SequenceGroup的路径
                    string parentDir = ModuleUtils.GetParentDirectory(testProjectPath);
                    sequenceGroupDir = $"{parentDir}{sequenceGroup.Name}{Path.DirectorySeparatorChar}";
                    sequenceGroup.Info.SequenceGroupFile = $"{sequenceGroupDir}{sequenceGroup.Name}.{CommonConst.SequenceFileExtension}";
                    sequenceGroup.Info.SequenceParamFile = $"{sequenceGroupDir}{sequenceGroup.Name}.{CommonConst.SequenceDataFileExtension}";
                }
                // 如果文件夹不存在，则创建
                if (!Directory.Exists(sequenceGroupDir))
                {
                    CreateDirectory(sequenceGroupDir);
                }
                string sequenceGroupPath = sequenceGroup.Info.SequenceGroupFile;
                string parameterPath = sequenceGroup.Info.SequenceParamFile;
                
                SequenceGroupLocationInfo locationInfo = new SequenceGroupLocationInfo()
                {
                    Name = sequenceGroup.Name,
                    SequenceFilePath = ModuleUtils.GetRelativePath(sequenceGroupPath, testProjectPath),
                    ParameterFilePath = ModuleUtils.GetRelativePath(parameterPath, testProjectPath)
                };
                testProject.SequenceGroupLocations.Add(locationInfo);
            }
        }

        private void CreateDirectory(string directory)
        {
            // 过滤磁盘驱动器路径
            Regex diskDriverRegex = new Regex(@"^((?:[a-zA-Z]:\\)|(?:/\w+/))");
            Match diskDriverMatch = diskDriverRegex.Match(directory);
            if (!diskDriverMatch.Success)
            {
                return;
            }
            StringBuilder currentDirCache = new StringBuilder(directory.Length);
            string diskDriverValue = diskDriverMatch.Groups[1].Value;
            currentDirCache.Append(diskDriverValue);
            char delim = Path.DirectorySeparatorChar;
            string[] dirElements = directory.Substring(diskDriverValue.Length).Split(delim);
            foreach (string dirElement in dirElements)
            {
                if (string.IsNullOrWhiteSpace(dirElement))
                {
                    continue;
                }
                currentDirCache.Append(dirElement).Append(delim);
                if (!Directory.Exists(currentDirCache.ToString()))
                {
                    Directory.CreateDirectory(currentDirCache.ToString());
                }
            }
        }

        public void UpdateSequenceGroupPathInfo(ISequenceGroup sequenceGroup, string seqFilePath)
        {
            sequenceGroup.Info.SequenceGroupFile = seqFilePath;
            sequenceGroup.Info.SequenceParamFile = ModuleUtils.GetParameterFilePath(seqFilePath);
        }

        // 配置SequenceGroupInfo中序列文件和参数文件的路径为相对路径
        public void SetInfoPathToRelative(ITestProject testProject)
        {
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                string sequenceAbsolutePath = sequenceGroup.Info.SequenceGroupFile;
                sequenceGroup.Info.SequenceGroupFile = ModuleUtils.GetFileName(sequenceAbsolutePath);
                sequenceGroup.Info.SequenceParamFile = ModuleUtils.GetRelativePath(sequenceGroup.Info.SequenceParamFile,
                    sequenceAbsolutePath);
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
        
        // 配置TestProject中SequenceGroupInfo的序列文件和参数文件的路径为绝对路径
        public void SetInfoPathToAbsolute(TestProject testProject, string testProjectPath)
        {
            ISequenceGroupCollection sequenceGroups = testProject.SequenceGroups;
            SequenceGroupLocationInfoCollection locations = testProject.SequenceGroupLocations;
            for (int i = 0; i < sequenceGroups.Count; i++)
            {
                sequenceGroups[i].Info.SequenceGroupFile = ModuleUtils.GetAbsolutePath(locations[i].SequenceFilePath,
                    testProjectPath);
                sequenceGroups[i].Info.SequenceParamFile = ModuleUtils.GetAbsolutePath(locations[i].ParameterFilePath,
                    testProjectPath);
            }
        }

        // 配置SequenceGroupInfo中序列文件和参数文件的路径为绝对路径
        public void SetInfoPathToAbsolute(ISequenceGroupInfo sequenceGroupInfo, string sequenceFullPath)
        {
            sequenceGroupInfo.SequenceGroupFile = ModuleUtils.GetAbsolutePath(sequenceGroupInfo.SequenceGroupFile, sequenceFullPath);
            sequenceGroupInfo.SequenceParamFile = ModuleUtils.GetAbsolutePath(sequenceGroupInfo.SequenceParamFile, sequenceFullPath);
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
            _availableDirs.Insert(_seqDirIndex, seqDir);
            SetAssembliesToAbsolutePath(testProject.Assemblies);
            _availableDirs.RemoveAt(_seqDirIndex);

            ISequenceGroupCollection sequenceGroups = testProject.SequenceGroups;
            foreach (ISequenceGroup sequenceGroup in sequenceGroups)
            {
                string seqGroupDir = ModuleUtils.GetParentDirectory(sequenceGroup.Info.SequenceGroupFile);
                _availableDirs.Insert(_seqDirIndex, seqGroupDir);
                try
                {
                    SetAssembliesToAbsolutePath(sequenceGroup.Assemblies);
                }
                finally
                {
                    _availableDirs.RemoveAt(_seqDirIndex);
                }
            }
        }

        public void SetAssembliesToAbsolutePath(ISequenceGroup sequenceGroup, string filePath)
        {
            string seqDir = ModuleUtils.GetParentDirectory(filePath);
            _availableDirs.Insert(_seqDirIndex, seqDir);
            try
            {
                SetAssembliesToAbsolutePath(sequenceGroup.Assemblies);
            }
            finally
            {
                _availableDirs.RemoveAt(_seqDirIndex);
            }
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
            foreach (string availableDir in _availableDirs)
            {
                _pathCache.Clear();
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

        private bool IsRelativePath(string path)
        {
            return !_absolutePathRegex.IsMatch(path);
        }
    }
}