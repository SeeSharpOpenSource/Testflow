using System;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class AssemblyInfo : IAssemblyInfo
    {
        public AssemblyInfo()
        {
            this.AssemblyName = string.Empty;
            this.Path = string.Empty;
            this.Available = false;
            this.Version = string.Empty;
        }

        public string AssemblyName { get; set; }

        public string Path { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        public bool Available { get; set; }

        public string Version { get; set; }

        public override bool Equals(object item)
        {
            IAssemblyInfo assemblyInfo = item as IAssemblyInfo;
            if (null == assemblyInfo)
            {
                return false;
            }
            return AssemblyName.Equals(assemblyInfo.AssemblyName) && Version.Equals(assemblyInfo.Version);
        }

        public override int GetHashCode()
        {
            return $"{AssemblyName}.{Version}".GetHashCode();
        }
    }
}