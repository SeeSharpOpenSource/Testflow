using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class TypeData : ITypeData
    {
        public TypeData()
        {
            this.AssemblyName = string.Empty;
            this.Namespace = string.Empty;
            this.Name = string.Empty;
        }

        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public string FullName => $"{AssemblyName}_{Namespace}.{Name}";

        public override bool Equals(object obj)
        {
            ITypeData typeData = obj as ITypeData;
            if (null == typeData)
            {
                return false;
            }
            return this.AssemblyName.Equals(typeData.AssemblyName) && this.Namespace.Equals(typeData.Namespace) &&
                   Name.Equals(typeData.Name);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        #region 序列化声明及反序列化构造

        public TypeData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillSerializationInfo(info, this);
        }

        #endregion
    }
}