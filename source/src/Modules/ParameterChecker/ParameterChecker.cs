using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.Usr;
using Testflow.Data.Description;

namespace Testflow.ParameterChecker
{
    public class ParameterChecker : IParameterChecker
    {
        private Modules.IComInterfaceManager _comInterfaceManager;
        public IModuleConfigData ConfigData { get; set; }

        #region 初始化
        public void DesigntimeInitialize()
        {
            _comInterfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;
        }

        public void RuntimeInitialize()
        {
            _comInterfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;
        }

        public void Dispose()
        {
            _comInterfaceManager?.Dispose();
        }

        public void ApplyConfig(IModuleConfigData configData)
        {

        }
        #endregion

        #region 参数检查
        /// <summary>
        /// 检查function instance
        /// 1. 检查是否为null：静态方法可以为null，不然报错
        /// 2. 在arr里面寻找对应的variable，调用CheckPropertyType
        /// </summary>
        /// <param name="function"> function信息 </param>
        /// <param name="arr"> 要检查的Sequence/SequenceGroup集合 </param>
        /// <returns> warnList (注：虽只会有一条错误信息，但为了上级代码整洁与一致性，返回List)</returns>
        /// 
        private IList<IWarningInfo> CheckInstance(IFunctionData function, ISequenceFlowContainer[] arr)
        {
            string instance = function.Instance;
            ITypeData classType = function.ClassType;
            IList<IWarningInfo> warnList = new List<IWarningInfo>();

            if(string.IsNullOrEmpty(instance))
            {
                //如果是静态方法，则不用检查,返回空的List
                if (function.Type == FunctionType.StaticFunction || function.Type == FunctionType.StaticPropertySetter) { }
                else
                {
                    warnList.Add(new WarningInfo()
                    {
                        WarnCode = WarnCode.ParameterDataNotAvailable,
                        Infomation = "Methods that are not static require instant to be not null"
                    });                 
                }
                return warnList;
            }

            IWarningInfo warnInfo = null;
            foreach (ISequenceFlowContainer cont in arr)
            {
                warnInfo = CheckPropertyType(cont, instance, classType);
                //表示没有错误信息，成功找到variable并通过
                if (warnInfo == null)
                {
                    return warnList;
                }
                //表示找到variable并且type不合
                if (warnInfo.WarnCode == WarnCode.TypeInvalid)
                {
                    warnList.Add(warnInfo);
                    return warnList;
                }
                //表示没找到Variable，继续循环去找
                if (warnInfo.WarnCode == WarnCode.VariableDNE)
                {
                    //continue;
                }
            }
            //最终还是没找到
            warnList.Add(warnInfo);
            return warnList;
        }

        /// <summary>
        /// 检查function return
        /// 1. 检查是否为null：静态方法可以为null，不然报错
        /// 2. 在arr里面寻找对应的variable，调用CheckPropertyType
        /// </summary>
        /// <param name="function"> function信息 </param>
        /// <param name="arr"> 要检查的Sequence/SequenceGroup集合 </param>
        /// <returns> warnList (注：虽只会有一条错误信息，但为了上级代码整洁与一致性，返回List)</returns>
        private IList<IWarningInfo> CheckReturn(IFunctionData function, ISequenceFlowContainer[] arr)
        {
            IList<IWarningInfo> warnList = new List<IWarningInfo>();
            //先判断返回类型是不是void, void则不检查
            if (function.ReturnType == null)
            {
                return warnList;
            }

            IWarningInfo warnInfo = null;
            foreach (ISequenceFlowContainer cont in arr)
            {
                warnInfo = CheckPropertyType(cont, function.Return, function.ReturnType.Type);
                //表示没有错误信息，成功找到variable并通过
                if (warnInfo == null)
                {
                    return warnList;
                }
                //表示找到variable并且type不合
                if (warnInfo.WarnCode == WarnCode.TypeInvalid)
                {
                    warnList.Add(warnInfo);
                    return warnList;
                }
                //表示没找到Variable，继续循环去找
                if (warnInfo.WarnCode == WarnCode.VariableDNE)
                {
                    //continue;
                }
            }
            //最终还是没找到
            //返回空的list
            //todo 找不到variable，没事，用户可以不用variable接收返回值？
            return warnList;
        }

        private IList<IWarningInfo> CheckParameterData(IFunctionData function, ISequenceFlowContainer[] arr)
        {
            IList<IWarningInfo> warningList = new List<IWarningInfo>();

            if(function.Parameters == null)
            {
                return warningList;
            }

            //检查一个个参数
            for (int n = 0; n < function.Parameters.Count; n++)
            {
                //取得当前检查的参数
                IParameterData parameterData = function.Parameters[n];
                IArgument parameterType = function.ParameterType[n];

                ///////////////////////////////////
                switch (parameterData.ParameterType)
                {
                    //还没输入参数值
                    //todo这里好像不对？
                    case ParameterType.NotAvailable:
                        if (function.Type != FunctionType.InstancePropertySetter && function.Type != FunctionType.StaticPropertySetter)
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

                        if(parameterType.VariableType == VariableType.Enumeration)
                        {
                            //获取assembly信息里的enumeration字典
                            //字典键：namespace.class
                            //字典值：string array含有枚举项
                            IComInterfaceDescription description = _comInterfaceManager.GetComInterfaceByName($"{parameterType.Type.AssemblyName}");
                            if(description == null)
                            {
                                warningList.Add(new WarningInfo()
                                {
                                    WarnCode = WarnCode.ParameterTypeAssemblyInvalid,
                                    Infomation = $"Could not find assembly {parameterType.Type.AssemblyName} for parameterType {parameterType.Name}"
                                });
                                break;
                            }

                            IDictionary<string,string[]> eCollection = description.Enumerations;
                            string[] e = null;
                            if (!eCollection.TryGetValue($"{parameterType.Type.Namespace}.{parameterType.Type.Name}", out e))
                            {
                                warningList.Add(new WarningInfo()
                                {
                                    WarnCode = WarnCode.EnumClassFault,
                                    Infomation = $"Could not find enumeration class {parameterType.Type.Namespace}.{parameterType.Type.Name}"
                                });
                                break;
                            }
                            if (!e.Contains(parameterData.Value))
                            {
                                warningList.Add(new WarningInfo()
                                {
                                    WarnCode = WarnCode.EnumDNE,
                                    Infomation = $"Could not find enumeration {parameterData.Value} in class {parameterType.Type.Namespace}.{parameterType.Type.Name}"
                                });
                                break;
                            }
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
                        //todo 如果说paremeter类型是value， 可能要Convertor.ValueConvertor.CheckValue（parameterType.Type.Name, Variable.Value）
                        //那要再写个GetValue去找相同名字的variable的value

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
                                break;
                            }
                            //表示没找到Variable，继续循环去找
                            if (warnInfo.WarnCode == WarnCode.VariableDNE)
                            {
                                //continue;
                            }
                        }
                        //最终还是没找到
                        if(warnInfo != null)
                        {
                            warningList.Add(warnInfo);
                        }
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
                    IList<IWarningInfo> stepWarningList = new List<IWarningInfo>();
                    IFunctionData function = step.Function;

                    //检查Instance
                    stepWarningList = stepWarningList.Concat(CheckInstance(function, arr)).ToList();

                    //检查Return
                    stepWarningList = stepWarningList.Concat(CheckReturn(function, arr)).ToList();

                    //检查Parameter
                    stepWarningList = stepWarningList.Concat(CheckParameterData(function, arr)).ToList();

                    //所有stepWarningInfo的step都是这个循环的step
                    foreach (IWarningInfo warnInfo in stepWarningList)
                    {
                        warnInfo.SequenceStep = step;
                    }
                    warningList = warningList.Concat(stepWarningList).ToList();
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
            #region 检查setup
            IList<IWarningInfo> setUpWarnList = new List<IWarningInfo>();

            setUpWarnList = CheckSteps(testProject.SetUp.Steps, new ISequenceFlowContainer[] { testProject.SetUp, testProject });
            foreach (IWarningInfo warnInfo in setUpWarnList)
            {
                warnInfo.Sequence = testProject.SetUp;
            }
            #endregion

            #region 检查teardown
            IList<IWarningInfo> tearDownWarnList = new List<IWarningInfo>();
            tearDownWarnList = CheckSteps(testProject.TearDown.Steps, new ISequenceFlowContainer[] { testProject.TearDown, testProject });
            foreach (IWarningInfo warnInfo in tearDownWarnList)
            {
                warnInfo.Sequence = testProject.TearDown;
            }
            #endregion

            return setUpWarnList.Concat(tearDownWarnList).ToList();
        }

        /// <summary>
        /// 校验SequenceGroup内模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// <returns>检查过程中出现的告警信息</returns>
        public IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup)
        {
            _comInterfaceManager.GetComponentInterfaces(sequenceGroup.Assemblies);
            IList<IWarningInfo> warnList = new List<IWarningInfo>();

            #region 检查setUp
            IList<IWarningInfo> setUpWarnList = CheckSteps(sequenceGroup.SetUp.Steps, new ISequenceFlowContainer[] { sequenceGroup.SetUp, sequenceGroup });
            foreach (IWarningInfo warnInfo in setUpWarnList)
            {
                warnInfo.Sequence = sequenceGroup.SetUp;
                warnInfo.SequenceGroup = sequenceGroup;
            }
            warnList = warnList.Concat(setUpWarnList).ToList();
            #endregion

            #region 检查teardown
            IList<IWarningInfo> tearDownWarnList = CheckSteps(sequenceGroup.TearDown.Steps, new ISequenceFlowContainer[] { sequenceGroup.TearDown, sequenceGroup });
            foreach (IWarningInfo warnInfo in tearDownWarnList)
            {
                warnInfo.Sequence = sequenceGroup.TearDown;
                warnInfo.SequenceGroup = sequenceGroup;
            }
            warnList = warnList.Concat(tearDownWarnList).ToList();
            #endregion

            #region 检查sequence
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                IList<IWarningInfo> sequenceWarnList = CheckSteps(sequence.Steps, new ISequenceFlowContainer[] { sequence, sequenceGroup });
                foreach (IWarningInfo warnInfo in sequenceWarnList)
                {
                    warnInfo.Sequence = sequence;
                    warnInfo.SequenceGroup = sequenceGroup;
                }
                warnList = warnList.Concat(sequenceWarnList).ToList();
            }
            #endregion

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

            #region 检查/赋予variable类型
            ITypeData type = variable.Type;
            //如果variable type为null，则赋checktype给它
            //todo need to check
            if(type == null)
            {
                variable.Type = checkType;
            }

            //如果入参为varname.property1.property2，则从varname中用模块取得属性type
            if (str.Length > 1)
            {
                type = _comInterfaceManager.GetPropertyType(type, variableString.Substring(variableString.IndexOf('.') + 1));
            }

            //比较type
            if (!type.Equals(checkType))
            {
                return new WarningInfo()
                {
                    WarnCode = WarnCode.TypeInvalid,
                    Infomation = $"Variable {variableString} data type different from paramater type."
                };
            }
            #endregion

            #region 检查值
            //检查空参
            //todo
            //如为空参则无报错信息？？
            //估计不能空参，当传入值的时候，然后你给个空参就应该报错，
            //那么我这个method里面应该再接收一个bool的值，看看能不能改
            if (String.IsNullOrEmpty(variable.Value))
            {
                return null;
            }

            //如果变量是值类型，
            //检查值是否与类型相符
            if (variable.VariableType == VariableType.Value)
            {
                if (!Convertor.ValueConvertor.CheckValue(variable.Type.Name, variable.Value))
                {
                    return new WarningInfo()
                    {
                        WarnCode = WarnCode.TypeInvalid,
                        Infomation = $"Variable {variableString} data type and its value invalid."
                    };
                }
            }

            //todo 判断variable如果是类类型，value实例是否跟type相符
            //if(variable.VariableType == VariableType.Class)
            #endregion

            return null;
        }
    }
    #endregion
}
