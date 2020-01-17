using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            ValidateExpressionTokens(testflowHome, expressionTokens);
            return new ExpressionOperatorCollection(expressionTokens);
        }

        private void ValidateExpressionTokens(string testflowHome, ExpressionTokenCollection expressionTokens)
        {
            string libraryDir = $"{testflowHome}{CommonConst.LibraryDir}{Path.DirectorySeparatorChar}";
            Type calculatorBaseType = typeof(IExpressionCalculator);
            string calculatorName = string.Empty;
            try
            {
                foreach (ExpressionOperatorInfo expressionToken in expressionTokens)
                {
                    calculatorName = expressionToken.Name;
                    string assemblyDir = libraryDir + expressionToken.Assembly;
                    Assembly assembly = Assembly.LoadFrom(assemblyDir);
                    Type calculatorType = assembly.GetType(expressionToken.ClassName);
                    if (!calculatorType.IsSubclassOf(calculatorBaseType))
                    {
                        TestflowRunner.GetInstance().LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession,
                            $"Invalid expression calculator:{expressionToken.ClassName}.");
                        I18N i18N = I18N.GetInstance(Constants.I18nName);
                        throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                            i18N.GetFStr("InvalidCalculator", expressionToken.ClassName));
                    }
                    expressionToken.CalculationClass = (IExpressionCalculator) Activator.CreateInstance(calculatorType);
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