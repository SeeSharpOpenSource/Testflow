using System;
using System.Threading;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.SequenceManager.Serializer;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager
{
    public class SequenceManager : ISequenceManager
    {
        private static SequenceManager _instance = null;
        private static object _instLock = new object();
        private readonly TypeMaintainer _typeMaintainer;

        public SequenceManager()
        {
            if (null != _instance)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(CommonErrorCode.InternalError, i18N.GetStr("InstAlreadyExist"));
            }
            lock (_instLock)
            {
                Thread.MemoryBarrier();
                if (null != _instance)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowRuntimeException(CommonErrorCode.InternalError, i18N.GetStr("InstAlreadyExist"));
                }
                I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_sequence_zh", "i18n_sequence_en")
                {
                    Name = Constants.I18nName
                };
                I18N.InitInstance(i18NOption);

                this.ConfigData = null;
                this.Version = string.Empty;
                _typeMaintainer = new TypeMaintainer();
                _instance = this;
            }
        }

        public IModuleConfigData ConfigData { get; set; }
        public string Version { get; set; }

        public void RuntimeInitialize()
        {

        }

        public void DesigntimeInitialize()
        {

        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this.ConfigData = configData;
            this.Version = configData.GetProperty<string>(Constants.VersionName);
        }

        public ITestProject CreateTestProject()
        {
            TestProject testProject = new TestProject()
            {
                ModelVersion = ConfigData.GetProperty<string>(Constants.VersionName)
            };
            return testProject;
        }

        public ISequenceGroup CreateSequenceGroup()
        {
            SequenceGroup sequenceGroup = new SequenceGroup();
            sequenceGroup.Info.Version = Version;
            return sequenceGroup;
        }

        public ISequence CreateSequence()
        {
            return new Sequence();
        }

        public ISequenceStep CreateSequenceStep()
        {
            return new SequenceStep();
        }

        public IArgument CreateArugment()
        {
            return new Argument();
        }

        public IFunctionData CreateFunctionData(IFuncInterfaceDescription funcInterface)
        {
            FunctionData functionData = new FunctionData();
            functionData.Initialize(funcInterface);
            return functionData;
        }

        public ILoopCounter CreateLoopCounter()
        {
            return new LoopCounter();
        }

        public IRetryCounter CreateRetryCounter()
        {
            return new RetryCounter();
        }

        public ISequenceGroupParameter CreateSequenceGroupParameter()
        {
            return new SequenceGroupParameter();
        }

        public ISequenceParameter CreateSequenceParameter()
        {
            return new SequenceParameter();
        }

        public ISequenceStepParameter CreateSequenceStepParameter()
        {
            return new SequenceStepParameter();
        }

        public IParameterData CreateParameterData(IArgument argument)
        {
            return new ParameterData();
        }

        public ITypeData CreateTypeData()
        {
            return new TypeData();
        }

        public IVariable CreateVarialbe()
        {
            return new Variable();
        }

        public IAssemblyInfo CreateAssemblyInfo()
        {
            return new AssemblyInfo();
        }

        public void Serialize(ITestProject testProject, SerializationTarget target, params string[] param)
        {
            switch (target)
            {
                case SerializationTarget.File:
                    _typeMaintainer.VerifyVariableTypes(testProject);
                    _typeMaintainer.RefreshUsedAssemblyAndType(testProject);
                    SequenceSerializer.Serialize(param[0], testProject as TestProject);
                    break;
                case SerializationTarget.DataBase:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public void Serialize(ISequenceGroup sequenceGroup, SerializationTarget target, params string[] param)
        {
            switch (target)
            {
                case SerializationTarget.File:
                    _typeMaintainer.VerifyVariableTypes(sequenceGroup);
                    _typeMaintainer.RefreshUsedAssemblyAndType(sequenceGroup);
                    SequenceSerializer.Serialize(param[0], sequenceGroup as SequenceGroup);
                    break;
                case SerializationTarget.DataBase:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        public ITestProject LoadTestProject(SerializationTarget source, params string[] param)
        {
            bool forceLoad = false;
            if (param.Length > 1 && null != param[1])
            {
                bool.TryParse(param[1], out forceLoad);
            }
            switch (source)
            {
                case SerializationTarget.File:
                    return SequenceDeserializer.LoadTestProject(param[0], forceLoad, this.ConfigData);
                    break;
                case SerializationTarget.DataBase:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public ISequenceGroup LoadSequenceGroup(SerializationTarget source, params string[] param)
        {
            bool forceLoad = false;
            if (param.Length > 1 && null != param[1])
            {
                bool.TryParse(param[1], out forceLoad);
            }
            switch (source)
            {
                case SerializationTarget.File:
                    return SequenceDeserializer.LoadSequenceGroup(param[0], forceLoad, this.ConfigData);
                    break;
                case SerializationTarget.DataBase:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public void LoadParameter(ISequenceGroup sequenceGroup, bool forceLoad, params string[] param)
        {
            SequenceGroup sequenceGroupObj = sequenceGroup as SequenceGroup;
            sequenceGroupObj.RefreshSignature();
            SequenceDeserializer.LoadParameter(sequenceGroupObj, param[0], forceLoad);
        }

        public string RuntimeSerialize(ITestProject testProject)
        {
            _typeMaintainer.VerifyVariableTypes(testProject);
            _typeMaintainer.RefreshUsedAssemblyAndType(testProject);
            return SequenceSerializer.ToJson(testProject as TestProject);
        }

        public string RuntimeSerialize(ISequenceGroup sequenceGroup)
        {
            _typeMaintainer.VerifyVariableTypes(sequenceGroup);
            _typeMaintainer.RefreshUsedAssemblyAndType(sequenceGroup);
            return SequenceSerializer.ToJson(sequenceGroup as SequenceGroup);
        }

        public ITestProject RuntimeDeserializeTestProject(string testProjectStr)
        {
            return SequenceDeserializer.LoadTestProjectFromJson(testProjectStr);
        }

        public ISequenceGroup RuntimeDeserializeSequenceGroup(string sequenceGroupStr)
        {
            return SequenceDeserializer.LoadSequenceGroupFromJson(sequenceGroupStr);
        }

        public void Dispose()
        {
            _instance = null;
        }
    }
}
