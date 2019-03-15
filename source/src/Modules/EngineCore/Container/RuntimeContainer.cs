using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.Logger;
using Testflow.Utility.I18nUtil;

namespace Testflow.EngineCore.Container
{
    /// <summary>
    /// 测试运行时容器
    /// </summary>
    internal abstract class RuntimeContainer
    {
        public static void CreateContainer(ISequenceFlowContainer sequence, int index, RuntimePlatform platform,
            ModuleGlobalInfo globalInfo, params object[] extraParam)
        {

        }

        protected readonly ISequenceFlowContainer Sequence;

        protected RuntimeContainer(ISequenceFlowContainer sequence, ModuleGlobalInfo globalInfo)
        {
            ISequenceGroup sequenceGroup = sequence as ISequenceGroup;
            // 如果是SequenceGroup并且还有入参，则必须和包含上级TestProject一起运行
            if (null != sequenceGroup && null == sequenceGroup.Parent && 0 != sequenceGroup.Arguments.Count)
            {
                ModuleUtility.LogAndRaiseDataException(LogLevel.Error, "SequenceGroup with input arguments cannot run with out test project.", 
                    ModuleErrorCode.SequenceDataError, null, "UnexistArgumentSource");
            }


            this.Sequence = sequence;
        }

        public virtual void Initialize()
        {
            
        }

        public abstract void Start();

        public abstract void Stop();
    }
}