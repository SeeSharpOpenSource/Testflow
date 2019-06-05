using System;
using System.Collections.Generic;
using Testflow.Data;
using Testflow.Data.Description;

namespace Testflow.ComInterfaceManager.Data
{
    [Serializable]
    public class ComInterfaceDescription : IComInterfaceDescription
    {
        public ComInterfaceDescription()
        {
            this.Classes = new List<IClassInterfaceDescription>(20);
            this.VariableTypes = new List<ITypeData>(20);
            this.TypeDescriptions = new List<ITypeDescription>(20);
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int ComponentId { get; set; }
        public string Signature { get; internal set; }
        public IAssemblyInfo Assembly { get; set; }
        public IList<IClassInterfaceDescription> Classes { get; }
        public IList<ITypeData> VariableTypes { get; set; }
        public List<ITypeDescription> TypeDescriptions { get; set; }
        public string Category { get; set; }
    }
}