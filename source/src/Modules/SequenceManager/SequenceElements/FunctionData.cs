using System;
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
            this.ParameterType = null;
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
        public ITypeData ClassType { get; set; }

        public int ClassTypeIndex { get; set; }
        public IArgumentCollection ParameterType { get; set; }
        public IParameterDataCollection Parameters { get; set; }
        public string Instance { get; set; }
        public string Return { get; set; }
        public IArgument ReturnType { get; set; }
        public IFuncInterfaceDescription Description { get; set; }

        public IFunctionData Clone()
        {
            ArgumentCollection parameterType = new ArgumentCollection();
            Common.Utility.CloneCollection(ParameterType, parameterType);

            ParameterDataCollection parameters = new ParameterDataCollection();
            Common.Utility.CloneCollection(Parameters, parameters);

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
    }
}