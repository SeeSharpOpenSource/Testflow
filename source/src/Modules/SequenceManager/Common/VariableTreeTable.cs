using System.Collections.Generic;
using System.Linq;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.Common
{
    internal class VariableTreeTable
    {
        private readonly List<IVariableCollection> _variableStack;
        private readonly IArgumentCollection _arguments;

        public VariableTreeTable(IArgumentCollection argumentses)
        {
            _variableStack = new List<IVariableCollection>(10);
            _arguments = argumentses;
        }

        public void Push(IVariableCollection variables)
        {
            _variableStack.Add(variables);
        }

        public void Pop()
        {
            _variableStack.RemoveAt(_variableStack.Count - 1);
        }

        public IVariable GetVariable(string variableName)
        {
            for (int i = _variableStack.Count - 1; i >= 0; i++)
            {
                IVariable variable = _variableStack[i].FirstOrDefault(item => item.Name.Equals(variableName));
                if (null != variable)
                {
                    return variable;
                }
            }
            return null;
        }

        public IArgument GetArgument(string variableName)
        {
            return _arguments?.FirstOrDefault(item => item.Name.Equals(variableName));
        }

        public void Clear()
        {
            _variableStack.Clear();
        }
    }
}