using System;
using System.Collections.Generic;
using Testflow.Data;
using Testflow.Data.Description;

namespace Testflow.ComInterfaceManager.Data
{
    [Serializable]
    public class TypeDescription : ITypeDescription
    {
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VariableType Kind { get; set; }
        public string Category { get; set; }
        public string[] Enumerations { get; set; }
    }
}