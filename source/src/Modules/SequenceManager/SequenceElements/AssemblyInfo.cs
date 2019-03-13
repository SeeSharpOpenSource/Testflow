using System;
using System.Runtime.Serialization;
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
        [RuntimeSerializeIgnore]
        public bool Available { get; set; }

        [RuntimeSerializeIgnore]
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

        #region 序列化声明及反序列化构造

        public AssemblyInfo(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillSerializationInfo(info, this);
        }

        #endregion
    }
}