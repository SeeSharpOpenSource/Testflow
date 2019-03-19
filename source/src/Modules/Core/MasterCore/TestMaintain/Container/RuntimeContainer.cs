using System;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.TestMaintain.Container
{
    /// <summary>
    /// 测试运行时容器
    /// </summary>
    internal abstract class RuntimeContainer : IDisposable
    {
        public static RuntimeContainer CreateContainer(ISequenceFlowContainer sequence, int index, RuntimePlatform platform,
            ModuleGlobalInfo globalInfo, params object[] extraParam)
        {
            switch (platform)
            {
                case RuntimePlatform.Clr:
                    return new AppDomainRuntimeContainer(sequence, globalInfo, extraParam);
                    break;
                case RuntimePlatform.Common:
                    return new ProcessRuntimeContainer(sequence, globalInfo, extraParam);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        protected readonly ISequenceFlowContainer Sequence;

        protected RuntimeContainer(ISequenceFlowContainer sequence, ModuleGlobalInfo globalInfo)
        {
            ISequenceGroup sequenceGroup = sequence as ISequenceGroup;
            // 如果是SequenceGroup并且还有入参，则必须和包含上级TestProject一起运行
            if (null != sequenceGroup && null == sequenceGroup.Parent && 0 != sequenceGroup.Arguments.Count)
            {
                ModuleUtility.LogAndRaiseDataException(LogLevel.Error, "SequenceGroup with input arguments cannot run with out test project.", ModuleErrorCode.SequenceDataError, null, "UnexistArgumentSource");
            }
            this.Sequence = sequence;
        }

        public abstract void Initialize();

        public abstract void Dispose();
    }
}