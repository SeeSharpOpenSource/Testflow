using Newtonsoft.Json;
using System;
using System.Threading;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepCallBackEntity : StepTaskEntityBase
    {
        private object[] Params;
        private string FullName;
        private int CallBackId;

        public StepCallBackEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            this.Params = new object[step.Function.Parameters?.Count ?? 0];
        }

        //method/Constructor Info
        public override void GenerateInvokeInfo()
        {
            //todo 做异步的时候再把Method加载进来，让slave运行
            //同步则无需加载method信息，因为会在master的CallBackProcessor里加载执行
        }

        // 改变StepData.Function.Parameters：如果是variable，则变为运行时$格式
        public override void InitializeParamsValues()
        {
            IArgumentCollection argumentInfos = StepData.Function.ParameterType;
            IParameterDataCollection parameters = StepData.Function.Parameters;
            for (int i = 0; i < argumentInfos.Count; i++)
            {
                string paramValue = parameters[i].Value;
                if (parameters[i].ParameterType == ParameterType.Value)
                {
                    Params[i] = Context.TypeInvoker.CastValue(argumentInfos[i].Type, paramValue);
                }
                else
                {
                    // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                    string variableName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                    IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                    if (null == variable)
                    {
                        Context.LogSession.Print(LogLevel.Error, SequenceIndex,
                            $"Unexist variable '{variableName}' in sequence data.");
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                            Context.I18N.GetFStr("UnexistVariable", variableName));

                    }
                    // 将变量的值保存到Parameter中
                    string varFullName = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
                    parameters[i].Value = ModuleUtils.GetFullParameterVariableName(varFullName, parameters[i].Value);
                    Params[i] = null;
                }
            }
            //todo 做同步的时候再添加对返回值的处理，代码参照StepExecutionEntity的InitializeParamsValue
        }

        //发送CallBackMessage至queue，让CallBackProcessor接受
        public void SendCallBackMessage()
        {
            CallBackId = Context.CallBackEventManager.GetCallBackId();

            IFunctionData function = StepData.Function;
            FullName = function.ClassType.Namespace + "." + function.ClassType.Name + "." + function.MethodName;
            CallBackMessage callBackMessage;

            //无参数
            if (function.Parameters == null || function.Parameters.Count == 0)
            {
                callBackMessage = new CallBackMessage(FullName, Context.SessionId, CallBackId)
                {
                    Type = MessageType.CallBack,
                };
            }
            else
            {
                //运行前实时获取参数信息
                SetVariableParamValue();
                callBackMessage = new CallBackMessage(FullName, Context.SessionId, CallBackId, getStringParams(function))
                {
                    Type = MessageType.CallBack,
                };
            }
            Context.UplinkMsgProcessor.SendMessage(callBackMessage, false);
        }

        //参数转成字符串
        private string[] getStringParams(IFunctionData function)
        {
            string[] stringParams = new string[Params.Length];
            for (int n = 0; n < Params.Length; n++)
            {
                stringParams[n] = function.ParameterType[n].VariableType == VariableType.Class
                ? JsonConvert.SerializeObject(Params[n])
                : Params[n].ToString();
            }
            return stringParams;
        }

        private void ExecuteCallBack(bool forceInvoke)
        {
            SendCallBackMessage();
            //取得阻塞event
            AutoResetEvent block = Context.CallBackEventManager.AcquireBlockEvent(CallBackId);
            //阻塞10秒
            //超时就抛出异常
            if (block.WaitOne(Constants.ThreadAbortJoinTime) == false)
            {
                this.Result = StepResult.Failed;
                throw new TaskFailedException(SequenceIndex, "CallBack has exceeded waiting Time", FailedType.RuntimeError);
            }

            //没超时，就获得消息
            CallBackMessage callBackMsg = Context.CallBackEventManager.GetMessageDisposeBlock(CallBackId);
            //回调成功
            if (callBackMsg.SuccessFlag)
            {
                this.Result = StepResult.Pass;
            }
            //回调不成功
            else
            {
                this.Result = StepResult.Failed;
                // 抛出强制失败异常
                throw new TaskFailedException(SequenceIndex, "CallBack failed", FailedType.RuntimeError);
            }
        }

        //todo 未考虑循环的事，如果循环，注意forceInvoke
        protected override void InvokeStep(bool forceInvoke)
        {
            this.Result = StepResult.Error;
            switch (StepData.Behavior)
            {
                case RunBehavior.Normal:
                    ExecuteCallBack(forceInvoke);
                    break;
                case RunBehavior.Skip:
                    this.Result = StepResult.Skip;
                    break;
                case RunBehavior.ForceSuccess:
                    try
                    {
                        ExecuteCallBack(forceInvoke);
                    }
                    catch (TaskFailedException ex)
                    {
                        this.Result = StepResult.Failed;
                        Context.LogSession.Print(LogLevel.Warn, SequenceIndex, ex,
                            "Execute failed but force success.");
                    }
                    break;
                case RunBehavior.ForceFailed:
                    ExecuteCallBack(forceInvoke);
                    this.Result = StepResult.Failed;
                    // 抛出强制失败异常
                    throw new TaskFailedException(SequenceIndex, FailedType.ForceFailed);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 因为Variable的值在整个过程中会变化，所以需要在运行前实时获取
        private void SetVariableParamValue()
        {
            IParameterDataCollection parameters = StepData.Function.Parameters;
            if (null == parameters)
            {
                return;
            }
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].ParameterType == ParameterType.Variable)
                {
                    // 获取变量值的名称，该名称为变量的运行时名称，其值在InitializeParamValue方法里配置
                    string variableName = ModuleUtils.GetVariableNameFromParamValue(parameters[i].Value);
                    // 使用变量名称获取变量当前对象的值
                    object variableValue = Context.VariableMapper.GetParamValue(variableName, parameters[i].Value);
                    // 根据ParamString和变量对应的值配置参数。
                    Params[i] = ModuleUtils.GetParamValue(parameters[i].Value, variableValue);
                }
            }
        }
    }
}