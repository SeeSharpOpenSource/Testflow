using System;
using System.Collections.Generic;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class SessionExecutionModel
    {
        private readonly SlaveContext _context;

        private readonly SequenceExecutionModel _setUp;

        private readonly SequenceExecutionModel _tearDown;

        private readonly List<SequenceExecutionModel> _sequenceModels;

        public SessionExecutionModel(SlaveContext context)
        {
            this._context = context;

            ISequenceFlowContainer sequenceData = _context.Sequence;
            switch (context.SequenceType)
            {
                case RunnerType.TestProject:
                    ITestProject testProject = (ITestProject)sequenceData;
                    _setUp = new SequenceExecutionModel(testProject.SetUp, _context);
                    _tearDown = new SequenceExecutionModel(testProject.TearDown, _context);
                    _sequenceModels = new List<SequenceExecutionModel>(1);
                    break;
                case RunnerType.SequenceGroup:
                    ISequenceGroup sequenceGroup = (ISequenceGroup)sequenceData;
                    _setUp = new SequenceExecutionModel(sequenceGroup.SetUp, _context);
                    _tearDown = new SequenceExecutionModel(sequenceGroup.TearDown, _context);
                    _sequenceModels = new List<SequenceExecutionModel>(sequenceGroup.Sequences.Count);
                    foreach (ISequence sequence in sequenceGroup.Sequences)
                    {
                        _sequenceModels.Add(new SequenceExecutionModel(sequence, _context));
                    }
                    break;
                default:
                    throw new InvalidProgramException();
            }
        }

        public void Generate()
        {
            _setUp.Generate();
            _tearDown.Generate();
            foreach (SequenceExecutionModel sequenceModel in _sequenceModels)
            {
                sequenceModel.Generate();
            }
        }

        public void InvokeSetUp()
        {
            _setUp.Invoke();
        }

        public void InvokeTearDown()
        {
            _tearDown.Invoke();
        }

        public void InvokeSequence(int index)
        {
            _sequenceModels[index].Invoke();
        }
    }
}