using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Testflow.ConfigurationManager.Data;
using Testflow.Data.Expression;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ConfigurationManager
{
    public class PlatformConfigManager : IConfigurationManager
    {
        public PlatformConfigManager()
        {
            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_config_zh", "i18n_config_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N i18N = I18N.GetInstance(Constants.I18nName);

            string platformDir = Environment.GetEnvironmentVariable(CommonConst.EnvironmentVariable);
            if (string.IsNullOrWhiteSpace(platformDir) || !Directory.Exists(platformDir))
            {
                TestflowRunner.GetInstance().LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, 
                    $"Invalid environment variable:{CommonConst.EnvironmentVariable}");
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidEnvDir, i18N.GetStr("InvalidHomeVariable"));
            }
            this.ConfigData = new ModuleConfigData();
            string configFilePath = $"{platformDir}{Path.DirectorySeparatorChar}{Constants.ConfigFileDir}{Path.DirectorySeparatorChar}{Constants.ConfigFileName}";
            this.ConfigData.SetProperty(Constants.ConfigFile, configFilePath);
            this.GlobalInfo = ConfigData;
        }

        public IModuleConfigData ConfigData { get; set; }
        public void RuntimeInitialize()
        {
            GlobalConfigData globalConfigData = Initialize();
            TestflowRunner testflowRunner = TestflowRunner.GetInstance();

            this.ApplyConfig(globalConfigData.GetGlobalConfigData());
            testflowRunner.DataMaintainer.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.DataMaintain));
            testflowRunner.EngineController.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.EngineConfig));
            testflowRunner.SequenceManager.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.SequenceManage));
            testflowRunner.ResultManager.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.ResultManage));

            globalConfigData.Dispose();
        }

        public void DesigntimeInitialize()
        {
            GlobalConfigData globalConfigData = Initialize();
            TestflowRunner testflowRunner = TestflowRunner.GetInstance();
            this.ApplyConfig(globalConfigData.GetGlobalConfigData());
            testflowRunner.ComInterfaceManager.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.InterfaceLoad));
            testflowRunner.DataMaintainer.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.DataMaintain));
            testflowRunner.EngineController.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.EngineConfig));
            testflowRunner.SequenceManager.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.SequenceManage));
            testflowRunner.ResultManager.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.ResultManage));
            testflowRunner.ParameterChecker.ApplyConfig(globalConfigData.GetModuleConfigData(Constants.ParamCheck));

            this.ConfigData.Properties.Add("TestName", "");
            this.ConfigData.Properties.Add("TestDescription", "");
            this.ConfigData.Properties.Add("RuntimeHash", "");
            this.ConfigData.Properties.Add("RuntimeType", RuntimeType.Run);


            globalConfigData.Dispose();
        }

        private GlobalConfigData Initialize()
        {
            GlobalConfigData globalConfigData;
            using (ConfigDataLoader dataLoader = new ConfigDataLoader())
            {
                globalConfigData = dataLoader.Load(ConfigData.GetProperty<string>(Constants.ConfigFile));

                // 获取表达式符号信息并添加到SequenceManager和EngineCore的配置信息中
                IExpressionOperatorCollection expressionTokens = GetExpressionTokens(globalConfigData, dataLoader);
                globalConfigData.AddConfigItem(Constants.SequenceManage, "ExpressionTokens", expressionTokens);
                globalConfigData.AddConfigItem(Constants.EngineConfig, "ExpressionTokens", expressionTokens);
            }
            return globalConfigData;
        }

        private IExpressionOperatorCollection GetExpressionTokens(GlobalConfigData globalConfigData, ConfigDataLoader dataLoader)
        {
            string testflowHome = globalConfigData.GetConfigValue<string>(Constants.GlobalConfig, "TestflowHome");
            string expressionConfigFile = $"{testflowHome}{CommonConst.DeployDir}{Path.DirectorySeparatorChar}expressionconfig.xml";
            ExpressionTokenCollection expressionTokens = dataLoader.LoadExpressionTokens(expressionConfigFile);
            // 有效化ExpressTokens
            ValidateExpressionTokens(globalConfigData, expressionTokens);
            return new ExpressionOperatorCollection(expressionTokens);
        }

        private void ValidateExpressionTokens(GlobalConfigData configData, ExpressionTokenCollection expressionTokens)
        {
            List<string> availableDirs = new List<string>(5);
            availableDirs.AddRange(configData.GetConfigValue<string[]>(Constants.GlobalConfig, "WorkspaceDir"));
            availableDirs.Add(configData.GetConfigValue<string>(Constants.GlobalConfig, "PlatformLibDir"));
            availableDirs.Add(configData.GetConfigValue<string>(Constants.GlobalConfig, "DotNetLibDir"));
            availableDirs.Add(configData.GetConfigValue<string>(Constants.GlobalConfig, "DotNetRootDir"));

            Type calculatorBaseType = typeof(IExpressionCalculator);
            string calculatorName = string.Empty;
            try
            {
                foreach (ExpressionOperatorInfo expressionToken in expressionTokens)
                {
                    calculatorName = expressionToken.Name;
                    // 获取表达式计算类的对象
                    string assemblyDir = GetAssemblyPath(expressionToken.Assembly, availableDirs);
                    Type calculatorType = GetTargetType(assemblyDir, expressionToken.ClassName, calculatorBaseType);
                    expressionToken.CalculationClass = (IExpressionCalculator) Activator.CreateInstance(calculatorType);
                    // 获取表达式源数据对象
                    assemblyDir = GetAssemblyPath(expressionToken.SourceAssembly, availableDirs);
                    Type sourceDataType = GetTargetType(assemblyDir, expressionToken.SourceClassName, null);
                    expressionToken.SourceClassType = sourceDataType;
                }
            }
            catch (TestflowException)
            {
                throw;
            }
            catch (ApplicationException ex)
            {
                TestflowRunner.GetInstance().LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex,
                            $"Invalid expression token {calculatorName}");
                throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                    ex.Message, ex);
            }
        }

        private static Type GetTargetType(string assemblyDir, string className, Type baseType)
        {
            if (!File.Exists(assemblyDir))
            {
                TestflowRunner.GetInstance().LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession,
                    $"Assembly of type:{className} cannot be located.");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                    i18N.GetFStr("InvalidCalculator", className));
            }
            Assembly assembly = Assembly.LoadFrom(assemblyDir);
            Type targetType = assembly.GetType(className);
            if (null == targetType || (null != baseType && !targetType.IsSubclassOf(baseType)))
            {
                TestflowRunner.GetInstance().LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession,
                    $"Invalid expression calculator:{className}.");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                    i18N.GetFStr("InvalidCalculator", className));
            }
            return targetType;
        }

        private string GetAssemblyPath(string assemblyPath, List<string> availableDirs)
        {
            
            string path = assemblyPath;
            StringBuilder pathCache = new StringBuilder(200);
            string abosolutePath = null;
            // 如果文件名前面有分隔符则去掉
            while (path.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Substring(1, path.Length - 1);
            }
            // 转换为绝对路径时反向寻找，先.net库再平台库再用户库
            for (int i = availableDirs.Count - 1; i >= 0; i--)
            {
                pathCache.Clear();
                string availableDir = availableDirs[i];
                pathCache.Append(availableDir).Append(path);
                // 如果库存在则配置为绝对路径，然后返回
                if (File.Exists(pathCache.ToString()))
                {
                    abosolutePath = pathCache.ToString();
                    break;
                }
            }
            return abosolutePath;
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            if (null == this.ConfigData)
            {
                this.ConfigData = configData;
            }
            else
            {
                foreach (KeyValuePair<string, object> keyValuePair in configData.Properties)
                {
                    this.ConfigData.SetProperty(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public IPropertyExtendable GlobalInfo { get; set; }
        public void LoadConfigurationData()
        {
            // ignore
        }

        public void ApplyConfigData(IController controller)
        {
            // ignore
        }

        public void Dispose()
        {
            I18N.RemoveInstance(Constants.I18nName);
        }

    }
}