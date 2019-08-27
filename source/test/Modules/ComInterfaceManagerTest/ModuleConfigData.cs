using System;
using System.Collections.Generic;
using System.Text;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Usr;
using Testflow.Utility.Collections;
using Testflow.Utility.MessageUtil;

namespace Testflow.ComInterfaceManagerTest
{
    public class ModuleConfigData : IModuleConfigData
    {
        public ModuleConfigData()
        {
            Properties = new SerializableMap<string, object>(50);
        }

        public void InitExtendProperties()
        {
            Properties.Add("TestName", "Test instance name");
            Properties.Add("TestDescription", "Test instance description");

            Properties.Add("LogLevel", LogLevel.Info);
            Properties.Add("PlatformEncoding", Encoding.UTF8);
            Properties.Add("DotNetLibDir", "");
            Properties.Add("PlatformLibDir", "");
            Properties.Add("InstanceLibDir", "");
            Properties.Add("ModelVersion", "10.12.35");
            Properties.Add("FileEncoding", "utf-8");

            Properties.Add("TestGenReportInterval", 500);
            Properties.Add("TestRunReportInterval", 500);
            Properties.Add("EngineQueueFormat", FormatterType.Xml);
            Properties.Add("EngineSyncMessenger", true);
            Properties.Add("RuntimeType", RuntimeType.Debug);
            Properties.Add("MessageReceiveTimeout", 10000);
            Properties.Add("StatusUploadInterval", 5000);
            Properties.Add("ConnectionTimeout", 10000);
            Properties.Add("SyncTimeout", 30000);
            Properties.Add("TestTimeout", 3600000);
            Properties.Add("StopTimeout", 10000);
            Properties.Add("TestGenTimeout", 1200000);
            Properties.Add("AbortTimeout", 20000);

            Properties.Add("TestflowHome", Environment.GetEnvironmentVariable("TESTFLOW_HOME"));
            Properties.Add("WorkspaceDir", new string[] { Environment.GetEnvironmentVariable("TESTFLOW_WORKSPACE") });
            Properties.Add("EnablePerformanceMonitor", false.ToString());
            Properties.Add("DatabaseName", "testflowData.db3");


            this.Version = "3.5.6";
            this.Name = "Test Name";
        }

        public ISerializableMap<string, object> Properties { get; }
        public void SetProperty(string propertyName, object value)
        {
            Properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return Properties[propertyName];
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            return (TDataType) GetProperty(propertyName);
        }

        public Type GetPropertyType(string propertyName)
        {
            return Properties[propertyName].GetType();
        }

        public bool ContainsProperty(string propertyName)
        {
            return Properties.ContainsKey(propertyName);
        }

        public IList<string> GetPropertyNames()
        {
            return new List<string>(Properties.Keys);
        }

        public string Version { get; set; }
        public string Name { get; set; }
    }
}