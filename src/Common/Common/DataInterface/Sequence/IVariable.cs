using System;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    public interface IVariable
    {
        string Name { get; set; }
        IAssemblyDescription Assembly { get; set; }
        Type Type { get; set; }
        VariableType VariableType { get; set; }

    }
}