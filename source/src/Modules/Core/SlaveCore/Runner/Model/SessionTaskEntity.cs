using System;
using System.Collections.Generic;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class SessionTaskEntity
    {
        private readonly SlaveContext _context;

        private readonly SequenceTaskEntity _setUp;

        private readonly SequenceTaskEntity _tearDown;

        private readonly List<SequenceTaskEntity> _sequenceEntities;

        public int SequenceCount => _sequenceEntities.Count;

        public SessionTaskEntity(SlaveContext context)
        {
            this._context = context;

            ISequenceFlowContainer sequenceData = _context.Sequence;
            switch (context.SequenceType)
            {
                case RunnerType.TestProject:
                    ITestProject testProject = (ITestProject)sequenceData;
                    _setUp = new SequenceTaskEntity(testProject.SetUp, _context);
                    _tearDown = new SequenceTaskEntity(testProject.TearDown, _context);
                    _sequenceEntities = new List<SequenceTaskEntity>(1);
                    break;
                case RunnerType.SequenceGroup:
                    ISequenceGroup sequenceGroup = (ISequenceGroup)sequenceData;
                    _setUp = new SequenceTaskEntity(sequenceGroup.SetUp, _context);
                    _tearDown = new SequenceTaskEntity(sequenceGroup.TearDown, _context);
                    _sequenceEntities = new List<SequenceTaskEntity>(sequenceGroup.Sequences.Count);
                    foreach (ISequence sequence in sequenceGroup.Sequences)
                    {
                        _sequenceEntities.Add(new SequenceTaskEntity(sequence, _context));
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
            foreach (SequenceTaskEntity sequenceModel in _sequenceEntities)
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
            _sequenceEntities[index].Invoke();
        }

        /// <summary>
        /// 心跳包中填充状态
        /// </summary>
        public void FillSequenceInfo(StatusMessage message)
        {
            _setUp.FillStatusInfo(message);
            foreach (SequenceTaskEntity sequenceTaskEntity in _sequenceEntities)
            {
                sequenceTaskEntity.FillStatusInfo(message);
            }
            _tearDown.FillStatusInfo(message);
        }

        /// <summary>
        /// 全局失败后填充状态
        /// </summary>
        public void FillSequenceInfo(StatusMessage message, string errorMessage)
        {
            _setUp.FillStatusInfo(message, errorMessage);
            foreach (SequenceTaskEntity sequenceTaskEntity in _sequenceEntities)
            {
                sequenceTaskEntity.FillStatusInfo(message, errorMessage);
            }
            _tearDown.FillStatusInfo(message, errorMessage);
        }
    }
}