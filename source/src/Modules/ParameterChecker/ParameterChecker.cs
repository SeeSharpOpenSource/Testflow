using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.Usr;

namespace Testflow.ParameterChecker
{
    public class ParameterChecker : IParameterChecker
    {
        private Modules.IComInterfaceManager _comtInterfaceManager;
        public IModuleConfigData ConfigData { get; set; }

        #region 初始化
        public void DesigntimeInitialize()
        {
            _comtInterfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;
        }

        public void RuntimeInitialize()
        {
            _comtInterfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;
        }

        public void Dispose()
        {
            _comtInterfaceManager?.Dispose();
        }

        public void ApplyConfig(IModuleConfigData configData)
        {

        }
        #endregion

        #region 参数检查

        private IWarningInfo CheckInstance(string instance, ITypeData classType, ISequenceFlowContainer[] arr)
        {
            IWarningInfo warnInfo = null;
            foreach (ISequenceFlowContainer cont in arr)
            {
                warnInfo = CheckPropertyType(cont, instance, classType);
                //表示没有错误信息，成功找到variable并通过
                if (warnInfo == null)
                {
                    return null;
                }
                //表示找到variable并且type不合
                if (warnInfo.WarnCode == WarnCode.TypeInvalid)
                {
                    return warnInfo;
                }
                //表示没找到Variable，继续循环去找
                if (warnInfo.WarnCode == WarnCode.VariableDNE)
                {
                    //continue;
                }
            }
            //最终还是没找到
            return warnInfo;
        }

        private IWarningInfo CheckReturn(string ret, IArgument argument, ISequenceFlowContainer[] arr)
        {
            IWarningInfo warnInfo = null;

            foreach (ISequenceFlowContainer cont in arr)
            {
                warnInfo = CheckPropertyType(cont, ret, argument.Type);
                //表示没有错误信息，成功找到variable并通过
                if (warnInfo == null)
                {
                    return null;
                }
                //表示找到variable并且type不合
                if (warnInfo.WarnCode == WarnCode.TypeInvalid)
                {
                    return warnInfo;
                }
                //表示没找到Variable，继续循环去找
                if (warnInfo.WarnCode == WarnCode.VariableDNE)
                {
                    //continue;
                }
            }
            //最终还是没找到
            // 找不到variable，没事，用户可以不用variable接收返回值？
            return null;
        }

        private IList<IWarningInfo> CheckParameterData(IParameterDataCollection paramaterDataCollection, IArgumentCollection parameterTypeCollection, FunctionType functionType, ISequenceFlowContainer[] arr)
        {
            IList<IWarningInfo> warningList = new List<IWarningInfo>();
            //检查一个个参数
            for (int n = 0; n < paramaterDataCollection.Count; n++)
            {
                //取得当前检查的参数
                IParameterData parameterData = paramaterDataCollection[n];
                IArgument parameterType = parameterTypeCollection[n];

                switch (parameterData.ParameterType)
                {
                    //还没输入参数值
                    case ParameterType.NotAvailable:
                        if (functionType != FunctionType.InstancePropertySetter && functionType != FunctionType.StaticPropertySetter)
                        {
                            warningList.Add(new WarningInfo()
                            {
                                WarnCode = WarnCode.ParameterDataNotAvailable,
                                Infomation = $"Parameter {n} data unavailable"
                            });
                        }
                        break;

                    //输入参数值
                    //检查参数类型是不是值类型
                    case ParameterType.Value:
                        //不判断VariableType.Undefined
                        //如果为类类型，则报错
                        if (parameterType.VariableType == VariableType.Class || parameterType.VariableType == VariableType.Struct)
                        {
                            warningList.Add(new WarningInfo()
                            {
                                WarnCode = WarnCode.TypeInvalid,
                                Infomation = $"Parameter {n} data type invalid: parameter type is of class, but input is value"
                            });
                            break;
                        }
                        //判断值类型是符合的值类型吗
                        if (!Convertor.ValueConvertor.CheckValue(parameterType.Type.Name, parameterData.Value))
                        {
                            warningList.Add(new WarningInfo()
                            {
                                WarnCode = WarnCode.TypeInvalid,
                                Infomation = $"Parameter {n} data type invalid: failed to parse input into parameter type"
                            });
                        }
                        break;

                    case ParameterType.Variable:
                        //如果不为类类型，则报错
                        if (parameterType.VariableType != VariableType.Class && parameterType.VariableType != VariableType.Struct)
                        {
                            warningList.Add(new WarningInfo()
                            {
                                WarnCode = WarnCode.TypeInvalid,
                                Infomation = $"Parameter {n} data type invalid"
                            });
                            break;
                        }

                        IWarningInfo warnInfo = null;
                        foreach (ISequenceFlowContainer cont in arr)
                        {
                            warnInfo = CheckPropertyType(cont, parameterData.Value, parameterType.Type);
                            //表示没有错误信息，成功找到variable并通过
                            if (warnInfo == null) 
                            {
                                break;
                            }
                            //表示找到variable并且type不合
                            if (warnInfo.WarnCode == WarnCode.TypeInvalid)
                            {
                                warningList.Add(warnInfo);
                            }
                            //表示没找到Variable，继续循环去找
                            if (warnInfo.WarnCode == WarnCode.VariableDNE)
                            {
                                //continue;
                            }
                        }
                        //最终还是没找到
                        warningList.Add(warnInfo);
                        break;

                }
            }
            return warningList;
        }

        private IList<IWarningInfo> CheckSteps(ISequenceStepCollection stepCollection, params ISequenceFlowContainer[] arr)
        {
            IList<IWarningInfo> warningList = new List<IWarningInfo>();
            //检查step是不是function，如果hasSubStep，去检查子substep
            foreach (ISequenceStep step in stepCollection)
            {
                if (step.HasSubSteps == true)
                {
                    warningList = warningList.Concat(CheckSteps(step.SubSteps)).ToList();
                }
                // 检查function
                else
                {
                    IFunctionData function = step.Function;

                    //检查Instance
                    foreach(ISequenceFlowContainer cont in arr)
                    {
                        warningList.Add(CheckInstance(function.Instance, function.ClassType, arr));
                    }

                    //检查Return
                    //先判断返回类型是不是void, void则不检查
                    if (function.ReturnType != null)
                    {
                        foreach (ISequenceFlowContainer cont in arr)
                        {
                            warningList.Add(CheckReturn(function.Return, function.ReturnType, arr));
                        }
                    }

                    //检查Parameter
                    foreach (ISequenceFlowContainer cont in arr)
                    {
                        warningList.Concat((CheckParameterData(function.Parameters, function.ParameterType, function.Type, arr))).ToString();
                    }

                    //所有warningInfo的step都是这个循环的step
                    foreach (IWarningInfo warnInfo in warningList)
                    {
                        warnInfo.SequenceStep = step;
                    }
                }
            }


            return warningList;
        }

        /// <summary>
        /// 校验TestProject内模块的参数配置正确性
        /// </summary>
        /// <param name="testProject">待校验的序列工程</param>
        /// <returns>检查过程中出现的告警信息</returns>
        public IList<IWarningInfo> CheckParameters(ITestProject testProject)
        {
            //check setup
            IList<IWarningInfo> setUpWarnList = new List<IWarningInfo>();

            setUpWarnList = CheckSteps(testProject.SetUp.Steps, new ISequenceFlowContainer[] { testProject.SetUp, testProject });
            foreach (IWarningInfo warnInfo in setUpWarnList)
            {
                warnInfo.Sequence = testProject.SetUp;
            }

            //check teardown
            IList<IWarningInfo> tearDownWarnList = new List<IWarningInfo>();
            tearDownWarnList = CheckSteps(testProject.TearDown.Steps, new ISequenceFlowContainer[] { testProject.TearDown, testProject });
            foreach (IWarningInfo warnInfo in tearDownWarnList)
            {
                warnInfo.Sequence = testProject.TearDown;
            }

            return setUpWarnList.Concat(tearDownWarnList).ToList();
        }

        /// <summary>
        /// 校验SequenceGroup内模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// <returns>检查过程中出现的告警信息</returns>
        public IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup)
        {
            IList<IWarningInfo> warnList = new List<IWarningInfo>();
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                IList<IWarningInfo> sequenceWarnList = CheckSteps(sequence.Steps, new ISequenceFlowContainer[] { sequence, sequenceGroup }).ToList();
                foreach (IWarningInfo warnInfo in sequenceWarnList)
                {
                    warnInfo.Sequence = sequence;
                    warnInfo.SequenceGroup = sequenceGroup;
                }
                warnList = warnList.Concat(sequenceWarnList).ToList();
            }
            
            return warnList;
        }

        /// <summary>
        /// 校验Sequence模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// <param name="sequence">待校验的序列</param>
        /// <returns>检查过程中出现的告警信息</returns>
        public IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup, ISequence sequence)
        {
            IList<IWarningInfo> warnList = CheckSteps(sequence.Steps, new ISequenceFlowContainer[] { sequence, sequenceGroup });
            foreach (IWarningInfo warnInfo in warnList)
            {
                warnInfo.Sequence = sequence;
                warnInfo.SequenceGroup = sequenceGroup;
            }
            return warnList;
        }

        /// <summary>
        /// 校验某个变量和属性构成的字符串对应的类型是否和待校验类型一致
        /// </summary>
        /// <param name="parent">该次所在的SequenceGroup或TestProject或Sequence</param>
        /// <param name="variableString">变量的字符串，样式类似于varname.property1.property2</param>
        /// <param name="checkType">待检查是否匹配的类型</param>
        /// <returns>如果错误返回错误信息，如果正确返回true</returns>
        public IWarningInfo CheckPropertyType(ISequenceFlowContainer parent, string variableString, ITypeData checkType)
        {
            if (parent is ITestProject)
            {
                return CheckPropertyType(((ITestProject)parent).Variables, variableString, checkType);
            }
            if (parent is ISequenceGroup)
            {
                return CheckPropertyType(((ISequenceGroup)parent).Variables, variableString, checkType);
            }

            if (parent is ISequence)
            {
                return CheckPropertyType(((ISequence)parent).Variables, variableString, checkType);
            }
            throw new TestflowDataException(ModuleErrorCode.InvalidParent,"parent must be ITestProject, ISequenceGroup, ISequence");
        }

        private IWarningInfo CheckPropertyType(IVariableCollection variables, string variableString, ITypeData checkType)
        {
            //split
            string[] str = variableString.Split('.');

            //找相同名字的variable
            IVariable variable = variables.FirstOrDefault(item => item.Name.Equals(str[0]));

            //没有此名的variable
            if (variable == null)
            {
                return new WarningInfo()
                {
                    WarnCode = WarnCode.VariableDNE,
                    Infomation = $"Variable {variableString} does not exist."
                };
            }

            ITypeData type = variable.Type;
            //如果variable type为null，则赋checktype给它
            if(type == null)
            {
                variable.Type = checkType;
                return null;
            }

            //如果入参为varname.property1.property2，则从varname中用模块取得属性type
            if (str.Length > 1)
            {
                type = _comtInterfaceManager.GetPropertyType(variable.Type, variableString.Substring(variableString.IndexOf('.') + 1));
            }

            //比较type
            if (!type.Equals(checkType))
            {
                return new WarningInfo()
                {
                    WarnCode = WarnCode.TypeInvalid,
                    Infomation = $"Variable {variableString} data type invalid."
                };
            }
            return null;
        }

    }
    #endregion
}
