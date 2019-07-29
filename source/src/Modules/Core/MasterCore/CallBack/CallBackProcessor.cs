using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Usr;

namespace Testflow.MasterCore.CallBack
{
    internal class CallBackProcessor : IMessageHandler
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private Thread _callBackProcessThread;
        private IDictionary<string, MethodInfo> _callBackMethods;
        private IDictionary<string, IFunctionData> _callBackFunctions;
        private AssemblyInvoker _typeInvoker;

        public CallBackProcessor(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
            _callBackMethods = new Dictionary<string, MethodInfo>(Constants.DefaultRuntimeSize);
            _callBackFunctions = new Dictionary<string, IFunctionData>(Constants.DefaultRuntimeSize);
        }

        #region RuntimeEngine初始化调用，_callBackMethods, _callBackFunctions记录回调函数的所需信息
        public void Initialize(ISequenceFlowContainer sequenceData)
        {
            if (sequenceData is ITestProject)
            {
                ITestProject testProject = (ITestProject)sequenceData;
                //AssemblyInvoker加载程序集和类型
                _typeInvoker = new AssemblyInvoker(_globalInfo, testProject.Assemblies, testProject.TypeDatas);
                _typeInvoker.LoadAssemblyAndType();

                foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
                {
                    SequenceGroupFindCallBack(sequenceGroup);
                }
            }
            else
            {
                ISequenceGroup sequenceGroup = (ISequenceGroup)sequenceData;
                _typeInvoker = new AssemblyInvoker(_globalInfo, sequenceGroup.Assemblies, sequenceGroup.TypeDatas);
                _typeInvoker.LoadAssemblyAndType();

                SequenceGroupFindCallBack(sequenceGroup);
            }

        }

        private void SequenceGroupFindCallBack(ISequenceGroup sequenceGroup)
        {
            StepCollectionFindCallBack(sequenceGroup.SetUp.Steps);
            StepCollectionFindCallBack(sequenceGroup.TearDown.Steps);
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                StepCollectionFindCallBack(sequence.Steps);
            }
        }

        private void StepCollectionFindCallBack(ISequenceStepCollection steps)
        {
            foreach (ISequenceStep step in steps)
            {
                StepFindCallBack(step);
            }
        }

        private void StepFindCallBack(ISequenceStep step)
        {
            if (step.HasSubSteps)
            {
                StepCollectionFindCallBack(step.SubSteps);
            }
            else
            {
                if (step.Function.Type == Data.FunctionType.CallBack)
                {
                    IFunctionData function = step.Function;
                    string fullname = function.ClassType.Namespace + "." + function.ClassType.Name + "." + function.MethodName;
                    _callBackMethods.Add(fullname, _typeInvoker.GetMethod(function));
                    _callBackFunctions.Add(fullname, function);
                }
            }
        }
        #endregion

        //发送成功与否的消息
        private void SendMessage(CallBackMessage message, bool success)
        {
            CallBackMessage callbackMsg = new CallBackMessage(message.Name, message.Id, ((CallBackMessage)message).CallBackId)
            {
                SuccessFlag = success,
            };
            _globalInfo.MessageTransceiver.Send(callbackMsg);
        }

        //执行step，然后发送消息
        private void ProcessCallBack(CallBackMessage message)
        {
            string methodName = message.Name;
            List<string> arg = message.Args;
            bool success = false;
            try
            {
                if (!_callBackFunctions.ContainsKey(methodName) || !_callBackMethods.ContainsKey(methodName))
                {
                    throw new TestflowException(ModuleErrorCode.CallBackFunctionNameError, $"Unable to load function name {methodName}");
                }
                IFunctionData function = _callBackFunctions[methodName];
                IArgumentCollection parameterTypes = function.ParameterType;
                IParameterDataCollection parameters = function.Parameters;

                object[] obj = new object[arg.Count];

                for (int n = 0; n < arg.Count; n++)
                {
                    //undefined的情况应该不存在吧
                    //enum或者value
                    if (parameterTypes[n].VariableType == VariableType.Undefined)
                    {
                        //todo I18n
                        throw new TestflowException(ModuleErrorCode.IncorrectParamType, "Type should not be Undefined.");
                    }
                    if (parameterTypes[n].VariableType == VariableType.Value || parameterTypes[n].VariableType == VariableType.Enumeration)
                    {
                        obj[n] = _typeInvoker.CastValue(parameterTypes[n].Type, (arg[n]));
                    }
                    //class或者struct
                    else
                    {
                        obj[n] = JsonConvert.DeserializeObject(arg[n], _typeInvoker.GetType(parameterTypes[n].Type));
                    }

                }
                _callBackMethods[methodName].Invoke(null, obj);
                success = true;
                SendMessage(message, success);
            }
            catch (TestflowException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, 0, ex, $"CallBack has a TestflowException when trying to execute {methodName}");
                SendMessage(message, success);
                throw;
            }
            catch (ThreadAbortException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, 0, ex, $"thread {Thread.CurrentThread.Name} stopped abnormally when executing {methodName}");
                SendMessage(message, success);
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, 0, ex, $"CallBack has an Exception when executing {methodName}");
                SendMessage(message, success);
            }

        }

        //收到消息，处理
        public bool HandleMessage(MessageBase message)
        {
            this._callBackProcessThread = new Thread(() => ProcessCallBack((CallBackMessage)message))
            {
                Name = "CallBackProcess",
                IsBackground = true
            };
            _callBackProcessThread.Start();
            return true;
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }
    }
}