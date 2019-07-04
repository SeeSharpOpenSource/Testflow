using System;
using System.Collections.Generic;
using Testflow.Data;
using Testflow.Data.Description;

namespace Testflow.ComInterfaceManager.Data
{
    [Serializable]
    public class FunctionInterfaceDescription : IFuncInterfaceDescription
    {
        public FunctionInterfaceDescription()
        {
            this.Arguments = null;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public FunctionType FuncType { get; set; }
        public int ComponentIndex { get; set; }
        public ITypeData ClassType { get; set; }
        public bool IsGeneric { get; set; }
        public IArgumentDescription Return { get; set; }
        public IList<IArgumentDescription> Arguments { get; set; }
        public string Signature { get; set; }
        public string Category { get; set; }
    }
}