using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Testflow.EngineCore.Data
{
    public class SyncResourceInfo : ISerializable
    {
        public string Assembly { get; set; }

        public string Type { get; set; }

        public string Instance { get; set; }

        public string Method { get; set; }

        public List<string> MethodParam { get; set; }

        public bool IsTypeSync => null == Instance && null == Method;

        public bool IsInstanceSync => null != Instance && null == Method;

        public bool IsMethodSync => null != Instance && null != Method;

        public static SyncResourceInfo CreateTypeSync(string assembly, string type)
        {
            return new SyncResourceInfo()
            {
                Assembly = assembly,
                Type = type,
            };
        }

        public static SyncResourceInfo CreateInstanceSync(string assembly, string type, string instance)
        {
            return new SyncResourceInfo()
            {
                Assembly = assembly,
                Type = type,
                Instance = instance
            };
        }

        public static SyncResourceInfo CreateFunctionSync(string assembly, string type, string instance, string method, params string[] paramTypes)
        {
            return new SyncResourceInfo()
            {
                Assembly = assembly,
                Type = type,
                Instance = instance,
                Method = method,
                MethodParam = new List<string>(paramTypes)
            };
        }

        private SyncResourceInfo()
        {
            this.Assembly = null;
            this.Type = null;
            this.Instance = null;
            this.Method = null;
            this.MethodParam = null;
        }

        public SyncResourceInfo(SerializationInfo info, StreamingContext context)
        {
            this.Assembly = info.GetValue("Assembly", typeof(string)) as string;
            this.Type = info.GetValue("Type", typeof(string)) as string;
            this.Instance = info.GetValue("Instance", typeof(string)) as string;
            this.Method = info.GetValue("Method", typeof(string)) as string;
            this.MethodParam = info.GetValue("MethodParam", typeof(List<string>)) as List<string>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Assembly", Assembly);
            info.AddValue("Type", Type);
            if (null != Assembly)
            {
                info.AddValue("Instance", Instance);
            }
            if (null != Method)
            {
                info.AddValue("Method", Method);
            }
            if (null != MethodParam && MethodParam.Count > 0)
            {
                info.AddValue("MethodParam", MethodParam);
            }
        }
    }
}