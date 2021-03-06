﻿using System;
using System.Collections.Generic;
using Testflow.Data;
using Testflow.Data.Description;

namespace Testflow.ComInterfaceManager.Data
{
    [Serializable]
    public class ClassInterfaceDescription : IClassInterfaceDescription
    {
        public ClassInterfaceDescription()
        {
            this.Functions = new List<IFuncInterfaceDescription>(10);
            this.Kind = VariableType.Undefined;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ComponentIndex { get; set; }
        public int ClassId { get; set; }
        public ITypeData ClassType { get; set; }
        public bool IsStatic { get; set; }
        public VariableType Kind { get; set; }
        public ITypeDescription ClassTypeDescription { get; set; }
        public IList<IFuncInterfaceDescription> Functions { get; set; }
    }
}