using System;
using System.Collections.Generic;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;

namespace Testflow.SlaveCore.Data
{
    internal class VariableMapper : IDisposable
    {
        private readonly Dictionary<string, object> _variables;
        private readonly SlaveContext _context;

        public VariableMapper(SlaveContext context)
        {
            ISequenceFlowContainer sequenceData = context.Sequence;
            this._variables = new Dictionary<string, object>(512);
            this._context = context;
            if (context.SequenceType == RunnerType.TestProject)
            {
                ITestProject testProject = (ITestProject)sequenceData;
                AddVariables(testProject.Variables);
                AddVariables(testProject.SetUp.Variables);
                AddVariables(testProject.TearDown.Variables);
            }
            else
            {
                ISequenceGroup sequenceGroup = (ISequenceGroup)sequenceData;
                AddVariables(sequenceGroup.Variables);
                AddVariables(sequenceGroup.SetUp.Variables);
                AddVariables(sequenceGroup.TearDown.Variables);
                foreach (ISequence sequence in sequenceGroup.Sequences)
                {
                    AddVariables(sequence.Variables);
                }
            }
        }

        private void AddVariables(IVariableCollection variables)
        {
            int sessionId = _context.SessionId;
            foreach (IVariable variable in variables)
            {
                string variableName = CoreUtils.GetRuntimeVariableName(sessionId, variable);
                this._variables.Add(variableName, null);
            }
        }

        public void SetVariableValue(string variableName, object value)
        {
            this._variables[variableName] = value;
        }

        public object GetVariableValue(string variableName)
        {
            return this._variables[variableName];
        }

        public void Dispose()
        {
            foreach (object value in _variables.Values)
            {
                (value as IDisposable)?.Dispose();
            }
            _variables.Clear();
        }
    }
}