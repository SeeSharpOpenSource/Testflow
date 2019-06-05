using System;
using Testflow.Data;
using Testflow.Data.Description;

namespace Testflow.ComInterfaceManager.Data
{
    [Serializable]
    public class PropertyDescription : IPropertyDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public VariableType ArgumentType { get; set; }
        public ITypeData Type { get; set; }
        public string DefaultValue { get; set; }
    }
}