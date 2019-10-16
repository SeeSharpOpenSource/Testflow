using System;
using System.Runtime.Remoting.Contexts;
using Testflow.CoreCommon.Data;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Runner.Actuators
{
    internal abstract class ActuatorBase
    {
        public static ActuatorBase GetActuator(ISequenceStep stepData, SlaveContext context, int sequenceIndex)
        {
            if (stepData?.Function == null)
            {
                return new EmptyActuator(context, stepData, sequenceIndex);
            }
            switch (stepData.Function.Type)
            {
                case FunctionType.Constructor:
                case FunctionType.InstanceFunction:
                case FunctionType.StaticFunction:
                    return new FunctionActuator(stepData, context, sequenceIndex);
                    break;
                case FunctionType.Assertion:
                    return new AssertActuator(stepData, context, sequenceIndex);
                    break;
                case FunctionType.CallBack:
                    return new CallBackActuator(stepData, context, sequenceIndex);
                    break;
                case FunctionType.StaticPropertySetter:
                case FunctionType.InstancePropertySetter:
                    return new PropertySetterActuator(stepData, context, sequenceIndex);
                    break;
                default:
                    throw new InvalidOperationException();
                    break;
            }
        }

        protected ActuatorBase(ISequenceStep step, SlaveContext context, int sequenceIndex)
        {
            this.Context = context;
            this.Function = step?.Function;
            this.StepData = step;
            this.SequenceIndex = sequenceIndex;
            this.Return = null;
        }

        protected SlaveContext Context { get; }

        protected IFunctionData Function { get; }

        protected int SequenceIndex { get; }

        protected ISequenceStep StepData { get; }

        public object Return { get; protected set; }

        /// <summary>
        /// 生成调用信息
        /// </summary>
        protected abstract void GenerateInvokeInfo();

        /// <summary>
        /// 初始化常数参数值
        /// </summary>
        protected abstract void InitializeParamsValues();

        /// <summary>
        /// 生成运行器
        /// </summary>
        public void Generate()
        {
            this.GenerateInvokeInfo();
            this.InitializeParamsValues();
        }

        /// <summary>
        /// 调用序列的执行代码
        /// </summary>
        public abstract StepResult InvokeStep(bool forceInvoke);
    }
}