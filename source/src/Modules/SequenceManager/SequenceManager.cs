using System;
using System.Threading;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager
{
    public class SequenceManager : ISequenceManager
    {
        private static SequenceManager _instance = null;
        private static object _instLock = new object();

        public static SequenceManager GetInstance()
        {
            if (null != _instance)
            {
                return _instance;
            }
            lock (_instLock)
            {
                Thread.MemoryBarrier();
                if (null != _instance)
                {
                    return _instance;
                }
                _instance = new SequenceManager();
                return _instance;
            }
        }

        private SequenceManager()
        {
            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_sequence_zh", "i18n_sequence_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);

            this.ConfigData = null;
            this.Version = string.Empty;
        }

        public IModuleConfigData ConfigData { get; set; }
        public string Version { get; set; }

        public void RuntimeInitialize()
        {
            throw new System.NotImplementedException();
        }

        public void DesigntimeInitialize()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this.ConfigData = configData;
            this.Version = configData.GetProperty<string>(Constants.VersionName);
        }

        public ITestProject CreateTestProject()
        {
            throw new System.NotImplementedException();
        }

        public ISequenceGroup CreateSequenceGroup()
        {
            throw new System.NotImplementedException();
        }

        public ISequence CreateSequence()
        {
            throw new System.NotImplementedException();
        }

        public ISequenceStep CreateSequenceStep()
        {
            throw new System.NotImplementedException();
        }

        public IArgument CreateArugment()
        {
            throw new System.NotImplementedException();
        }

        public IFunctionData CreateFunctionData(IFuncInterfaceDescription funcInterface)
        {
            throw new System.NotImplementedException();
        }

        public ILoopCounter CreateLoopCounter()
        {
            throw new System.NotImplementedException();
        }

        public IRetryCounter CreateRetryCounter()
        {
            throw new System.NotImplementedException();
        }

        public ISequenceGroupParameter CreateSequenceGroupParameter()
        {
            throw new System.NotImplementedException();
        }

        public ISequenceParameter CreateSequenceParameter()
        {
            throw new System.NotImplementedException();
        }

        public ISequenceStepParameter CreateSequenceStepParameter()
        {
            throw new System.NotImplementedException();
        }

        public IParameterData CreateParameterData(IArgument argument)
        {
            throw new System.NotImplementedException();
        }

        public ITypeData CreateTypeData()
        {
            throw new System.NotImplementedException();
        }

        public IVariable CreateVarialbe()
        {
            throw new System.NotImplementedException();
        }

        public IAssemblyInfo CreateAssemblyInfo()
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ITestProject project, SerializationTarget target, params string[] param)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ISequenceGroup sequenceGroup, SerializationTarget target, params string[] param)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(SequenceGroup sequenceGroup, SerializationTarget target, params string[] param)
        {
            throw new System.NotImplementedException();
        }

        public ITestProject LoadTestProject(SerializationTarget source, params string[] param)
        {
            throw new System.NotImplementedException();
        }

        ISequenceGroup ISequenceManager.LoadSequenceGroup(SerializationTarget source, params string[] param)
        {
            return DeserializeSequenceGroup(source, param);
        }

        public void LoadParameter(ISequenceGroup sequenceGroup, bool forceLoad, params string[] param)
        {
            throw new NotImplementedException();
        }

        public SequenceGroup DeserializeSequenceGroup(SerializationTarget source, params string[] param)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
