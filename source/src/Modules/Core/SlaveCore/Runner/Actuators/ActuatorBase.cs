using System;
using System.Diagnostics;
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
            this.ExecutionTime = DateTime.MaxValue;
            this.ExecutionTicks = -1;
        }

        protected SlaveContext Context { get; }

        protected IFunctionData Function { get; }

        protected int SequenceIndex { get; }

        protected ISequenceStep StepData { get; }

        public object Return { get; protected set; }

        public DateTime ExecutionTime { get; protected set; }
        
        public long ExecutionTicks { get; protected set; }

        /// <summary>
        /// 当前运行所在运行器的逻辑ID。(因为是在县城内部控制调用逻辑，可以认为是某种协程)
        /// </summary>
        private int _coroutineId;

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
        public void Generate(int coroutineId)
        {
            this._coroutineId = coroutineId;
            this.GenerateInvokeInfo();
            this.InitializeParamsValues();
        }

        /// <summary>
        /// 开启运行计时
        /// </summary>
        public void StartTiming()
        {
            this.ExecutionTicks = -1;
            this.ExecutionTime = DateTime.Now;
            Context.TimingManager.StartTiming(_coroutineId);
        }

        /// <summary>
        /// 停止运行计时。为了保证性能，该方法还会在外部捕获异常的代码里执行
        /// </summary>
        public void EndTiming()
        {
            long ticks = Context.TimingManager.EndTiming(_coroutineId);
            if (this.ExecutionTicks <= -1)
            {
                this.ExecutionTicks = ticks;
            }
        }

        /// <summary>
        /// 重置计时，用于在未真正执行前重置运行时间，以防止出现非法的时间记录
        /// </summary>
        public void ResetTiming()
        {
            this.ExecutionTicks = -1;
            this.ExecutionTime = DateTime.Now;
        }

        /// <summary>
        /// 调用序列的执行代码
        /// </summary>
        public abstract StepResult InvokeStep(bool forceInvoke);
    }
}