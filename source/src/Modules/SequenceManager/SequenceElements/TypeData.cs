using System;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class TypeData : ITypeData
    {
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
    }
}