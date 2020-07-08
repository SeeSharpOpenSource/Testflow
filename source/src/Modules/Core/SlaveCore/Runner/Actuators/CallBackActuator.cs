using System;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.ExtensionBase.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Expression;
using Testflow.SlaveCore.Runner.Model;
using Testflow.Usr;
using Testflow.Usr.Common;

namespace Testflow.SlaveCore.Runner.Actuators
{
    internal class CallBackActuator : ActuatorBase
    {
        private object[] _params;
        private string FullName;
        private int CallBackId;
        private CallBackType callBackType;

        public CallBackActuator(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            this._params = new object[Function.Parameters?.Count ?? 0];
        }

        protected override void GenerateInvokeInfo()
        {
            MethodInfo methodInfo = Context.TypeInvoker.GetMethod(Function);
            if (null == methodInfo)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.RuntimeError,
                    Context.I18N.GetFStr("LoadFunctionFailed", Function.MethodName));
            }
            //判断同步异步
            callBackType = methodInfo.GetCustomAttribute<CallBackAttribute>().CallBackType;
        }

        // 改变StepData.Function.Parameters：如果是variable，则变为运行时$格式
        protected override void InitializeParamsValues()
        {
            IArgumentCollection argumentInfos = Function.ParameterType;
            IParameterDataCollection parameters = Function.Parameters;
            for (int i = 0; i < argumentInfos.Count; i++)
            {
                string paramValue = parameters[i].Value;
                switch (parameters[i].ParameterType)
                {
                    case ParameterType.Value:
                        _params[i] = Context.TypeInvoker.CastConstantValue(argumentInfos[i].Type, paramValue);
                        break;
                    case ParameterType.Variable:
                        // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                        string variableName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                        IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                        if (null == variable)
                        {
                            Context.LogSession.Print(LogLevel.Error, Context.SessionId,
                                $"Unexist variable '{variableName}' in sequence data.");
                            throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                Context.I18N.GetFStr("UnexistVariable", variableName));
                        }
                        // 将变量的值保存到Parameter中
                        string varFullName = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
                        parameters[i].Value = ModuleUtils.GetFullParameterVariableName(varFullName, parameters[i].Value);
                        _params[i] = null;
                        break;
                    case ParameterType.NotAvailable:
                        // 如果参数的修饰符为out，则可以不配置
                        if (argumentInfos[i].Modifier != ArgumentModifier.Out)
                        {
                            Context.LogSession.Print(LogLevel.Error, Context.SessionId,
                                $"The value of parameter '{argumentInfos[i].Name}' in step '{StepData.Name}' is not configured");
                            throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                    Context.I18N.GetFStr("UnconfiguredParam", argumentInfos[i].Name));
                        }
                        break;
                    case ParameterType.Expression:
                        ExpressionProcessor expProcessor =
                            Context.CoroutineManager.GetCoroutineHandle(CoroutineId).ExpressionProcessor;
                        int expIndex = expProcessor.CompileExpression(paramValue, StepData);
                        // 在参数数据中写入表达式索引
                        parameters[i].Value = expIndex.ToString();
                        break;
                    default:
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                Context.I18N.GetStr("InvalidParamVar"));
                        break;
                }
            }
            if (null != Function.ReturnType && CoreUtils.IsValidVaraible(Function.Return))
            {
                // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                string variableName = ModuleUtils.GetVariableNameFromParamValue(Function.Return);
                IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                if (null == variable)
                {
                    Context.LogSession.Print(LogLevel.Error, SequenceIndex,
                        $"Unexist variable '{variableName}' in sequence data.");
                    throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                        Context.I18N.GetFStr("UnexistVariable", variableName));
                }
//                _returnVar = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
            }
//            CommonStepDataCheck(_instanceVar);
        }

        //发送CallBackMessage至queue，让CallBackProcessor接受
        private void SendCallBackMessage()
        {
            CallBackId = Context.CallBackEventManager.GetCallBackId();

            IFunctionData function = Function;
            FullName = function.ClassType.Namespace + "." + function.ClassType.Name + "." + function.MethodName;
            CallBackMessage callBackMessage;

            //无参数
            if (function.Parameters == null || function.Parameters.Count == 0)
            {
                callBackMessage = new CallBackMessage(FullName, Context.SessionId, CallBackId)
                {
                    Type = MessageType.CallBack,
                    CallBackType = this.callBackType
                };
            }
            else
            {
                //运行前实时获取参数信息
                SetVariableParamValue();
                callBackMessage = new CallBackMessage(FullName, Context.SessionId, CallBackId, getStringParams(function))
                {
                    Type = MessageType.CallBack,
                    CallBackType = this.callBackType
                };
            }
            Context.UplinkMsgProcessor.SendMessage(callBackMessage, false);
        }

        //参数转成字符串
        private string[] getStringParams(IFunctionData function)
        {
            string[] stringParams = new string[_params.Length];
            for (int n = 0; n < _params.Length; n++)
            {
                stringParams[n] = function.ParameterType[n].VariableType == VariableType.Class
                ? JsonConvert.SerializeObject(_params[n])
                : _params[n].ToString();
            }
            return stringParams;
        }

        //todo 未考虑循环的事，如果循环，注意forceInvoke
        public override StepResult InvokeStep(bool forceInvoke)
        {
            StepResult result1 = StepResult.NotAvailable;
            // 开始计时
            StartTiming();
            SendCallBackMessage();
            #region 同步：等待master发回消息
            if (callBackType == CallBackType.Synchronous)
            {
                //取得阻塞event
                AutoResetEvent block = Context.CallBackEventManager.AcquireBlockEvent(CallBackId);
                //阻塞100秒
                //超时就抛出异常
                if (block.WaitOne(Constants.ThreadAbortJoinTime) == false)
                {
                    result1 = StepResult.Failed;
                    throw new TaskFailedException(SequenceIndex, "CallBack has exceeded waiting Time", FailedType.RuntimeError, ModuleErrorCode.EventTimeOut);
                }

                //没超时，就获得消息
                CallBackMessage callBackMsg = Context.CallBackEventManager.GetMessageDisposeBlock(CallBackId);
                //回调成功
                if (callBackMsg.SuccessFlag)
                {
                    result1 = StepResult.Pass;
                }
                //回调不成功
                else
                {
                    result1 = StepResult.Failed;
                    // 抛出强制失败异常
                    throw new TaskFailedException(SequenceIndex, "CallBack failed", FailedType.RuntimeError, ModuleErrorCode.UserForceFailed);
                }
            }
                #endregion
                #region 异步：不管master直接通过步骤
            else
            {
                result1 = StepResult.Pass;
            }
            // 停止计时
            EndTiming();
            #endregion
            StepResult result = result1;
            return result;
        }

        // 因为Variable的值在整个过程中会变化，所以需要在运行前实时获取
        private void SetVariableParamValue()
        {
            IArgumentCollection arguments = Function.ParameterType;
            IParameterDataCollection parameters = Function.Parameters;
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
                    // 根据ParamString和变量对应的值配置参数。
                    _params[i] = Context.VariableMapper.GetParamValue(variableName, parameters[i].Value,
                        arguments[i].Type);
                }
                else if (parameters[i].ParameterType == ParameterType.Expression)
                {
                    int expIndex = int.Parse(parameters[i].Value);
                    ExpressionProcessor expProcessor =
                        Context.CoroutineManager.GetCoroutineHandle(CoroutineId).ExpressionProcessor;
                    _params[i] = expProcessor.Calculate(expIndex, arguments[i].Type);
                }
            }
        }
    }
}