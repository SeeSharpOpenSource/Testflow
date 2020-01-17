using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.ConfigurationManager;
using Testflow.ConfigurationManager.Data;
using Testflow.Runtime;
using Testflow.Usr;
using Testflow.Utility.MessageUtil;

namespace Testflow.ConfigurationManagerTest
{
    [TestClass]
    public class ConfigFileCreator
    {
        [TestMethod]
        public void CreateConfigFile()
        {
            GlobalConfigData globalConfigData = new GlobalConfigData();
            globalConfigData.AddConfigRoot("GlobalConfig");
            globalConfigData.AddConfigItem("GlobalConfig", "PlatformEncoding", Encoding.UTF8.BodyName);
            globalConfigData.AddConfigItem("GlobalConfig", "PlatformLibDir", @"lib\");
//            globalConfigData.AddConfigItem("GlobalConfig", "WorkspaceDir", @"C:\Users\jingtao\Documents\TestflowWorkspace");
            globalConfigData.AddConfigItem("GlobalConfig", "DotNetVersion", @"v4.0.30319");
            globalConfigData.AddConfigItem("GlobalConfig", "LogLevel", LogLevel.Info);

            globalConfigData.AddConfigRoot("EngineCore");
            globalConfigData.AddConfigItem("EngineCore", "TestGenReportInterval", 500);
            globalConfigData.AddConfigItem("EngineCore", "TestRunReportInterval", 500);
            globalConfigData.AddConfigItem("EngineCore", "EngineQueueFormat", FormatterType.Json);
            globalConfigData.AddConfigItem("EngineCore", "EngineSyncMessenger", true);
            globalConfigData.AddConfigItem("EngineCore", "MessageReceiveTimeout", 10000);
            globalConfigData.AddConfigItem("EngineCore", "StatusUploadInterval", 500);
            globalConfigData.AddConfigItem("EngineCore", "ConnectionTimeout", 10000);
            globalConfigData.AddConfigItem("EngineCore", "SyncTimeout", 10000);
            globalConfigData.AddConfigItem("EngineCore", "TestTimeout", 1000000000);
            globalConfigData.AddConfigItem("EngineCore", "StopTimeout", 10000);
            globalConfigData.AddConfigItem("EngineCore", "TestGenTimeout", 5000);
            globalConfigData.AddConfigItem("EngineCore", "AbortTimeout", 10000);
            globalConfigData.AddConfigItem("EngineCore", "MessengerType", MessengerType.MSMQ);
            globalConfigData.AddConfigItem("EngineCore", "EnablePerformanceMonitor", false);
            globalConfigData.AddConfigItem("EngineCore", "NumericFormat", "0.#####");

            globalConfigData.AddConfigRoot("DataMaintain");
            globalConfigData.AddConfigItem("DataMaintain", "DatabaseName", "testflowData.db3");

            globalConfigData.AddConfigRoot("ResultManager");

            globalConfigData.AddConfigRoot("SequenceInfo");
            globalConfigData.AddConfigItem("SequenceInfo", "ModelVersion", "1.0.0.0");
            globalConfigData.AddConfigItem("SequenceInfo", "FileEncoding", Encoding.UTF8.ToString());

            globalConfigData.AddConfigRoot("ParameterCheck");
            globalConfigData.AddConfigRoot("InterfaceLoad");

            ConfigData configData = globalConfigData.GetConfigData();
            configData.ConfigVersion = "1.0.0.0";

            FileStream fileStream = new FileStream("config.xml", FileMode.Create);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof (ConfigData),
                new Type[] { typeof(ConfigData), typeof (ConfigBlock), typeof (ConfigItem)});
            xmlSerializer.Serialize(fileStream, configData);
        }

        [TestMethod]
        public void CreateExpressionConfigFile()
        {
            ExpressionTokenCollection tokenCollection = new ExpressionTokenCollection(10);
            tokenCollection.Add(new ExpressionOperatorInfo()
            {
                Name = "GetArrayElement",
                Description = "Get the element in specified subscript.",
                Symbol = "[]",
                FormatString = "{0}[{1}]",
                Assembly = "ExpressionFuncs.dll",
                ClassName = "Testflow.ExpressionFunctions.ArrayElementAcquirer",
                SourceAssembly = "mscorlib.dll",
                SourceClassName = "System.Array"
            });
            ExpressionOperatorConfiguration configData = new ExpressionOperatorConfiguration()
            {
                Version = "1.0.1.0",
                ExpressionOperators = tokenCollection
            };

            FileStream fileStream = new FileStream("expressionconfig.xml", FileMode.Create);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExpressionOperatorConfiguration),
                new Type[] { typeof(ExpressionOperatorInfo), typeof(ExpressionTokenCollection) });
            xmlSerializer.Serialize(fileStream, configData);
        }
    }
}