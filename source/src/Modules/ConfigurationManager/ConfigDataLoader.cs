using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using Testflow.ConfigurationManager.Data;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.ConfigurationManager
{
    internal class ConfigDataLoader : IDisposable
    {
        private readonly Dictionary<string, Func<string, object>> _valueConvertor;
        public ConfigDataLoader()
        {
            this._valueConvertor = new Dictionary<string, Func<string, object>>(10);
            _valueConvertor.Add(GetFullName(typeof(string)), strValue => strValue);
            _valueConvertor.Add(GetFullName(typeof(double)), strValue => double.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(float)), strValue => float.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(long)), strValue => long.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(int)), strValue => int.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(uint)), strValue => uint.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(short)), strValue => short.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(ushort)), strValue => ushort.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(char)), strValue => char.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(byte)), strValue => byte.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(bool)), strValue => bool.Parse(strValue));
        }

        public GlobalConfigData Load(string configFile)
        {
            ConfigData configData = GetConfigData(configFile);
            GlobalConfigData globalConfigData = GetGlobalConfigData(configData);
            AddExtraGlobalConfigData(globalConfigData);
            AddExtraRuntimeConfigData(globalConfigData);
            return globalConfigData;
        }

        private ConfigData GetConfigData(string configFile)
        {
            ConfigData configData;
            using (FileStream fileStream = new FileStream(configFile, FileMode.Open))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (ConfigData),
                    new Type[] {typeof (ConfigBlock), typeof (ConfigItem)});
                configData = xmlSerializer.Deserialize(fileStream) as ConfigData;
            }
            return configData;
        }

        private GlobalConfigData GetGlobalConfigData(ConfigData configData)
        {
            GlobalConfigData globalConfigData = new GlobalConfigData();
            foreach (ConfigBlock configBlock in configData.ModuleConfigData)
            {
                string blockName = configBlock.Name;
                globalConfigData.AddConfigRoot(blockName);
                foreach (ConfigItem configItem in configBlock.ConfigItems)
                {
                    Type valueType = Type.GetType(configItem.Type);
                    if (null == valueType)
                    {
                        valueType = Assembly.GetAssembly(typeof (IConfigurationManager)).GetType(configItem.Type);
                        if (null == valueType)
                        {
                            valueType = Assembly.GetAssembly(typeof (Messenger)).GetType(configItem.Type);
                            if (null == valueType)
                            {
                                I18N i18N = I18N.GetInstance(Constants.I18nName);
                                throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                                    i18N.GetFStr("CannotLoadType", configItem.Type));
                            }
                        }
                    }
                    object value;
                    if (valueType.IsEnum)
                    {
                        value = Enum.Parse(valueType, configItem.Value);
                    }
                    else if (valueType.IsValueType || valueType == typeof (string))
                    {
                        value = _valueConvertor[GetFullName(valueType)].Invoke(configItem.Value);
                    }
                    else if (valueType == typeof (Encoding))
                    {
                        value = Encoding.GetEncoding(configItem.Value);
                    }
                    else
                    {
                        I18N i18N = I18N.GetInstance(Constants.I18nName);
                        throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                            i18N.GetFStr("UnsupportedCast", configItem.Type));
                    }
                    globalConfigData.AddConfigItem(blockName, configItem.Name, value);
                }
            }
            return globalConfigData;
        }

        private void AddExtraRuntimeConfigData(GlobalConfigData globalConfigData)
        {
            globalConfigData.AddConfigItem(Constants.EngineConfig, "TestName", "TestInstance");
            globalConfigData.AddConfigItem(Constants.EngineConfig, "TestDescription", "");
            globalConfigData.AddConfigItem(Constants.EngineConfig, "RuntimeHash", "");
            globalConfigData.AddConfigItem(Constants.EngineConfig, "RuntimeType", RuntimeType.Run);
        }

        private void AddExtraGlobalConfigData(GlobalConfigData configData)
        {
            // 更新TestflowHome字段
            string homeDir = Environment.GetEnvironmentVariable(CommonConst.EnvironmentVariable);
            if (null != homeDir && !homeDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                homeDir += Path.DirectorySeparatorChar;
            }
            configData.AddConfigItem(Constants.GlobalConfig, "TestflowHome", homeDir);

            // 更新WorkspaceDir字段
            
            // 更新.NET运行时目录
            string dotNetVersion = configData.GetConfigValue<string>(Constants.GlobalConfig, "DotNetVersion");
            string runtimeDirectory = GetDotNetDir(dotNetVersion);
            configData.AddConfigItem(Constants.GlobalConfig, "DotNetLibDir", runtimeDirectory);
            
            // 更新Testflow平台默认库目录
            string platformDir = configData.GetConfigValue<string>(Constants.GlobalConfig, "PlatformLibDir");
            configData.SetConfigItem(Constants.GlobalConfig, "PlatformLibDir", $"{homeDir}{platformDir}");

            // 更新Testflow工作空间目录
            string workspaceDirs = Environment.GetEnvironmentVariable(CommonConst.WorkspaceVariable);
            if (string.IsNullOrWhiteSpace(workspaceDirs) || !Directory.Exists(workspaceDirs))
            {
                TestflowRunner.GetInstance().LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession,
                    $"Invalid environment variable:{CommonConst.WorkspaceVariable}");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidEnvDir, i18N.GetStr("InvalidHomeVariable"));
            }
            string[] workSpaceDirElems = workspaceDirs.Split(';');
            List<string> workspaceDirList = new List<string>(workSpaceDirElems.Length);
            foreach (string workSpaceDir in workSpaceDirElems)
            {
                if (string.IsNullOrWhiteSpace(workSpaceDir))
                {
                    continue;
                }
                string dirPath = workSpaceDir;
                if (!workSpaceDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    dirPath += Path.DirectorySeparatorChar;
                }
                workspaceDirList.Add(dirPath);
            }
            configData.SetConfigItem(Constants.GlobalConfig, "WorkspaceDir", workspaceDirList.ToArray());
        }

        private static string GetDotNetDir(string dotNetVersion)
        {
            const string netInstallationFldr = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\.NETFramework";
            const string netDxInstallationRoot = "InstallRoot";
            string frameworkRoot = (string)Registry.GetValue(netInstallationFldr, netDxInstallationRoot, null);
            if (string.IsNullOrWhiteSpace(frameworkRoot))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidEnvDir, i18N.GetStr("InvalidDotNetDir"));
            }
            string frameworkDir = $"{frameworkRoot}{dotNetVersion}{Path.DirectorySeparatorChar}";
            if (!Directory.Exists(frameworkDir))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidEnvDir, i18N.GetStr("InvalidDotNetDir"));
            }
            return frameworkDir;
        }

        private static string GetFullName(Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }

        public void Dispose()
        {
            _valueConvertor.Clear();
        }
    }
}