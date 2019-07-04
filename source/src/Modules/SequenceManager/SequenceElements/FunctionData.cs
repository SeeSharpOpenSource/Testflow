using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class FunctionData : IFunctionData
    {
        public FunctionData()
        {
            this.Type = FunctionType.StaticFunction;
            this.MethodName = string.Empty;
            this.ClassType = null;
            this.ClassTypeIndex = Constants.UnverifiedTypeIndex;
            this.ParameterType = new ArgumentCollection();
            this.Parameters = new ParameterDataCollection();
            this.Instance = string.Empty;
            this.Return = string.Empty;
            this.ReturnType = null;
            this.Description = null;
        }

        public FunctionType Type { get; set; }
        public string MethodName { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ITypeData ClassType { get; set; }

        public int ClassTypeIndex { get; set; }
        [RuntimeType(typeof(ArgumentCollection))]
        public IArgumentCollection ParameterType { get; set; }
        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeType(typeof(ParameterDataCollection))]
        public IParameterDataCollection Parameters { get; set; }
        public string Instance { get; set; }
        public string Return { get; set; }
        [RuntimeType(typeof(Argument))]
        public IArgument ReturnType { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public IFuncInterfaceDescription Description { get; set; }

        public IFunctionData Clone()
        {
            ArgumentCollection parameterType = new ArgumentCollection();
            ModuleUtils.CloneCollection(ParameterType, parameterType);

            ParameterDataCollection parameters = new ParameterDataCollection();
            ModuleUtils.CloneCollection(Parameters, parameters);

            FunctionData functionData = new FunctionData()
            {
                Type = this.Type,
                MethodName = this.MethodName,
                ClassType = this.ClassType,
                ClassTypeIndex = this.ClassTypeIndex,
                ParameterType = parameterType,
                Parameters = parameters,
                Instance = this.Instance,
                Return = this.Return,
                ReturnType = this.ReturnType.Clone(),
                Description = this.Description
            };
            return functionData;
        }

        public void Initialize(IFuncInterfaceDescription funcInterface)
        {
            ArgumentCollection argumentsTypes = new ArgumentCollection();
            foreach (IArgumentDescription argumentDescription in funcInterface.Arguments)
            {
                Argument argumentData = new Argument();
                argumentData.Initialize(argumentDescription);
                argumentsTypes.Add(argumentData);
            }

            ParameterDataCollection parameters = new ParameterDataCollection();
            foreach (IArgumentDescription argumentDescription in funcInterface.Arguments)
            {
                parameters.Add(new ParameterData());
            }

            Type = funcInterface.FuncType;
            MethodName = funcInterface.Name;
            ClassType = funcInterface.ClassType;
            Description = funcInterface;
            Instance = string.Empty;
            Parameters = parameters;
            ParameterType = argumentsTypes;

            if (null != funcInterface.Return)
            {
                Argument returnType = new Argument();
                returnType.Initialize(funcInterface.Return);
                ReturnType = returnType;
            }
        }

        #region 序列化声明及反序列化构造

        public FunctionData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillSerializationInfo(info, this);
        }

        #endregion
    }
}