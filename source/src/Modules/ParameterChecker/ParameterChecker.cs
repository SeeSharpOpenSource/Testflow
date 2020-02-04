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

        private IWarningInfo FindVariablesCheckPropertyType(ISequenceFlowContainer[] arr, string variableString, ITypeData CheckType, bool overwriteType)
        {
            IWarningInfo warnInfo = null;
            foreach (ISequenceFlowContainer cont in arr)
            {
                warnInfo = CheckPropertyType(cont, variableString, CheckType, overwriteType);
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
        public IWarningInfo CheckInstance(ISequenceStep Step, ISequenceFlowContainer[] arr, bool overwriteType)
        {
            IFunctionData function = Step.Function;

            if(string.IsNullOrEmpty(function.Instance))
            {
                //如果是静态方法，则不用检查,返回空的List
                if (function.Type == FunctionType.StaticFunction || function.Type == FunctionType.StaticPropertySetter) { return null; }
                else
                {
                    return new WarningInfo(){
                        //todo I18n
                        WarnCode = WarnCode.ParameterDataNotAvailable,
                        Infomation = $"\"{Step.Name}\" require instant to be not null"
                    };                 
                }
            }

            return FindVariablesCheckPropertyType(arr, function.Instance, function.ClassType, overwriteType);
        }

        /// <summary>
        /// 检查function return
        /// 1. 检查是否为null：静态方法可以为null，不然报错
        /// 2. 在arr里面寻找对应的variable，调用CheckPropertyType
        /// </summary>
        /// <param name="function"> function信息 </param>
        /// <param name="arr"> 要检查的Sequence/SequenceGroup集合 </param>
        /// <returns> warnList (注：虽只会有一条错误信息，但为了上级代码整洁与一致性，返回List)</returns>
        public IWarningInfo CheckReturn(ISequenceStep Step, ISequenceFlowContainer[] arr, bool overwriteType)
        {
            IFunctionData function = Step.Function;

            //先判断返回类型是不是void, void则不检查
            if (function.ReturnType == null)
            {
                if (!string.IsNullOrEmpty(function.Return))
                {
                    return new WarningInfo()
                    {
                        //todo I18n
                        WarnCode = WarnCode.ReturnNotEmpty,
                        Infomation = $"\"{Step.Name}\" function Return Type is void, but value not empty"
                    };
                }

                return null;
            }
            //空值也不检查
            else if (string.IsNullOrEmpty(function.Return))
            {
                return null;
            }

            return FindVariablesCheckPropertyType(arr, function.Return, function.ReturnType.Type, overwriteType);
        }

        public IList<IWarningInfo> CheckStep(ISequenceStep Step, ISequenceFlowContainer[] arr, bool overwriteType)
        {
            IList<IWarningInfo> warningList = new List<IWarningInfo>();
            IFunctionData function = Step.Function;

            #region 空function, 空step
            if(function == null)
            {
                return warningList;
            }
            #endregion

            IWarningInfo warnInfo = null;

            #region Instance
            warnInfo = CheckInstance(Step, arr, overwriteType);
            if(warnInfo != null)
            {
                warningList.Add(warnInfo);
            }
            #endregion

            #region Return
            warnInfo = CheckReturn(Step, arr, overwriteType);
            if (warnInfo != null)
            {
                warningList.Add(warnInfo);
            }
            #endregion

            #region Parameters
            if (function.Parameters == null) { }
            else for (int n = 0; n < function.Parameters.Count; n++)
                {
                    warnInfo = CheckParameterData(function, n, arr, overwriteType);
                    if (warnInfo != null)
                    {
                        warningList.Add(warnInfo);
                    }
                }
            #endregion

            foreach(IWarningInfo info in warningList)
            {
                info.SequenceStep = Step;
            }

            return warningList;
        }

        public IWarningInfo CheckParameterData(IFunctionData function, int index, ISequenceFlowContainer[] arr, bool overwriteType)
        {
            IParameterData parameterData = function.Parameters[index];
            IArgument parameterType = function.ParameterType[index];

            switch (parameterData.ParameterType)
            {
                //还没输入参数值
                //todo这里好像不对？
                case ParameterType.NotAvailable:
                    if (function.Type != FunctionType.InstancePropertySetter && function.Type != FunctionType.StaticPropertySetter)
                    {
                       return new WarningInfo()
                        {
                            WarnCode = WarnCode.ParameterDataNotAvailable,
                            Infomation = $"Parameter \"{parameterType.Name}\" not available"
                        };
                    }
                    return null;
                    break;
                //输入参数值
                //检查参数类型是不是值类型
                case ParameterType.Value:
                    //不判断VariableType.Undefined
                    //如果为类类型，则报错
                    if (parameterType.VariableType == VariableType.Class || parameterType.VariableType == VariableType.Struct)
                    {
                        return new WarningInfo()
                        {
                            WarnCode = WarnCode.TypeInvalid,
                            Infomation = $"Parameter \"{parameterType.Name}\" data type invalid: parameter type is of class, but input is value"
                        };
                    }

                    else if (parameterType.VariableType == VariableType.Enumeration)
                    {
                        //获取assembly信息里的enumeration字典
                        //字典键：namespace.class
                        //字典值：string array含有枚举项
                        IComInterfaceDescription description = _comInterfaceManager.GetComInterfaceByName(parameterType.Type.AssemblyName);
                        if (description == null)
                        {
                            return new WarningInfo()
                            {
                                WarnCode = WarnCode.ParameterTypeAssemblyInvalid,
                                Infomation = $"Could not find assembly {parameterType.Type.AssemblyName} for parameterType {parameterType.Name}"
                            };
                        }

                        IDictionary<string, string[]> eCollection = description.Enumerations;
                        string[] e = null;
                        if (!eCollection.TryGetValue($"{parameterType.Type.Namespace}.{parameterType.Type.Name}", out e))
                        {
                            return new WarningInfo()
                            {
                                WarnCode = WarnCode.EnumClassFault,
                                Infomation = $"Could not find enumeration class {parameterType.Type.Namespace}.{parameterType.Type.Name}"
                            };
                        }
                        if (!e.Contains(parameterData.Value))
                        {
                            return new WarningInfo()
                            {
                                WarnCode = WarnCode.EnumDNE,
                                Infomation = $"Could not find enumeration {parameterData.Value} in class {parameterType.Type.Namespace}.{parameterType.Type.Name}"
                            };
                        }

                    }
                    else            //parameterType.VariableType == VariableType.Value
                    {
                        //判断值类型是符合的值类型吗
                        if (!ValueConvertor.CheckValue(parameterType.Type.Name, parameterData.Value))
                        {
                            return new WarningInfo()
                            {
                                WarnCode = WarnCode.TypeInvalid,
                                Infomation = $"Parameter \"{parameterType.Name}\" data type invalid: failed to parse input into parameter type"
                            };
                        }
                    }
                    
                    return null;
                    break;
                case ParameterType.Variable:
                    return FindVariablesCheckPropertyType(arr, parameterData.Value, parameterType.Type, overwriteType);
                    break;
                    case ParameterType.Expression:
                    return null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid parameter value type.");
                    break;
            }
        }

        private IList<IWarningInfo> CheckSteps(ISequenceStepCollection stepCollection, bool overwriteType, params ISequenceFlowContainer[] arr)
        {
            IList<IWarningInfo> warningList = new List<IWarningInfo>();
            
            
            foreach (ISequenceStep step in stepCollection)
            {
                // 校验step
                if (null != step.Function)
                {
                    warningList = warningList.Concat(CheckStep(step, arr, overwriteType)).ToList();
                }
                //检查step是不是function，如果hasSubStep，去校验子substep
                if (step.HasSubSteps == true)
                {
                    warningList = warningList.Concat(CheckSteps(step.SubSteps, overwriteType, arr)).ToList();
                }
            }

            return warningList;
        }

        /// <summary>
        /// 校验TestProject内模块的参数配置正确性
        /// </summary>
        /// <param name="testProject">待校验的序列工程</param>
        /// <returns>检查过程中出现的告警信息</returns>
        public IList<IWarningInfo> CheckParameters(ITestProject testProject, bool overwriteType)
        {
            #region 检查setup
            IList<IWarningInfo> setUpWarnList = new List<IWarningInfo>();

            setUpWarnList = CheckSteps(testProject.SetUp.Steps, overwriteType, new ISequenceFlowContainer[] { testProject.SetUp, testProject });
            foreach (IWarningInfo warnInfo in setUpWarnList)
            {
                warnInfo.Sequence = testProject.SetUp;
            }
            #endregion

            #region 检查teardown
            IList<IWarningInfo> tearDownWarnList = new List<IWarningInfo>();
            tearDownWarnList = CheckSteps(testProject.TearDown.Steps, overwriteType, new ISequenceFlowContainer[] { testProject.TearDown, testProject });
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
        public IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup, bool overwriteType)
        {
            _comInterfaceManager.GetComponentInterfaces(sequenceGroup.Assemblies);
            IList<IWarningInfo> warnList = new List<IWarningInfo>();

            #region 检查setUp
            IList<IWarningInfo> setUpWarnList = CheckSteps(sequenceGroup.SetUp.Steps, overwriteType, new ISequenceFlowContainer[] { sequenceGroup.SetUp, sequenceGroup });
            foreach (IWarningInfo warnInfo in setUpWarnList)
            {
                warnInfo.Sequence = sequenceGroup.SetUp;
                warnInfo.SequenceGroup = sequenceGroup;
            }
            warnList = warnList.Concat(setUpWarnList).ToList();
            #endregion

            #region 检查teardown
            IList<IWarningInfo> tearDownWarnList = CheckSteps(sequenceGroup.TearDown.Steps, overwriteType, new ISequenceFlowContainer[] { sequenceGroup.TearDown, sequenceGroup });
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
                IList<IWarningInfo> sequenceWarnList = CheckSteps(sequence.Steps, overwriteType, new ISequenceFlowContainer[] { sequence, sequenceGroup });
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
        public IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup, ISequence sequence, bool overwriteType)
        {
            IList<IWarningInfo> warnList = CheckSteps(sequence.Steps, overwriteType, new ISequenceFlowContainer[] { sequence, sequenceGroup });
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
        public IWarningInfo CheckPropertyType(ISequenceFlowContainer parent, string variableString, ITypeData checkType, bool overwriteType)
        {
            if (parent is ITestProject)
            {
                return CheckPropertyType(((ITestProject)parent).Variables, variableString, checkType, overwriteType);
            }
            if (parent is ISequenceGroup)
            {
                return CheckPropertyType(((ISequenceGroup)parent).Variables, variableString, checkType, overwriteType);
            }

            if (parent is ISequence)
            {
                return CheckPropertyType(((ISequence)parent).Variables, variableString, checkType, overwriteType);
            }
            throw new TestflowDataException(ModuleErrorCode.InvalidParent,"parent must be ITestProject, ISequenceGroup, ISequence");
        }

        private IWarningInfo CheckPropertyType(IVariableCollection variables, string variableString, ITypeData checkType, bool overwriteType)
        {
            //split
            string[] str = variableString.Split('.');

            #region 寻找variable
            //找相同名字的variable
            IVariable variable = variables.FirstOrDefault(item => item.Name.Equals(str[0]));

            //没有此名的variable
            if (variable == null)
            {
                return new WarningInfo()
                {
                    WarnCode = WarnCode.VariableDNE,
                    Infomation = $"Variable \"{variableString}\" does not exist."
                };
            }
            #endregion

            #region 寻找/赋予variable类型
            ITypeData type = variable.Type;

            #region 入参为varname.property1.property2
            //从varname中用模块取得属性type
            if (str.Length > 1)
            {
                type = _comInterfaceManager.GetPropertyType(type, variableString.Substring(variableString.IndexOf('.') + 1));

                //todo 万一没找到这个属性就要给warningInfo.对上面用trycatch？
            }
            #endregion

            #region 入参为varname
            else
            {
                //如果variable type为null，则赋checktype给它
                if (type == null && overwriteType)
                {
                    type = checkType;
                }
            }
            #endregion

            #endregion

            #region 比较类型
            if (type == null)
            {
                return new WarningInfo()
                {
                    WarnCode = WarnCode.TypeInvalid,
                    Infomation = $"Variable {variableString} has Type null . Different from Type \"{checkType.Name}\" . May cause issues during runtime."
                };
            }
            else if (!type.Equals(checkType))
            {
                return new WarningInfo()
                {
                    WarnCode = WarnCode.TypeInvalid,
                    Infomation = $"Variable {variableString} Type \"{type.Name}\" different from Type \"{checkType.Name}\". May cause issues during runtime."
                };
            }
            #endregion

            #region //检查值
            //#region 空值或不为VariableType.Value
            //if (String.IsNullOrEmpty(variable.Value) || variable.VariableType != VariableType.Value)
            //{
                
            //}
            //#endregion
            //#region 有值且VariableType.Value
            //else
            //{
            //    if (!Convertor.ValueConvertor.CheckValue(checkType.Name, variable.Value))
            //    {
            //        return new WarningInfo()
            //        {
            //            WarnCode = WarnCode.TypeInvalid,
            //            Infomation = $"Variable {variableString} data type and its value invalid."
            //        };
            //    }
            //}
            //#endregion
            ////如果变量是值类型，
            ////检查值是否与类型相符
            //if (variable.VariableType == VariableType.Value)
            //{
                
            //}

            //todo 判断variable如果是类类型，value实例是否跟type相符
            //if()
            #endregion

            return null;
        }

        //todo I18n
        public IWarningInfo CheckVariableValue(ISequenceFlowContainer parent, string name)
        {
            IVariable variable;

            #region TestProject
            if(parent is ITestProject)
            {
                variable = ((ITestProject)parent).Variables.FirstOrDefault(item => item.Name.Equals(name));
                if(variable == null)
                {
                    throw new TestflowDataException(ModuleErrorCode.VariableNotFound, $"Variable {name} not found in TestProject {parent.Name}");
                }
                IWarningInfo warnInfo = CheckVariableValue(variable);
                if(warnInfo != null)
                {
                    warnInfo.SequenceGroup = null;
                    warnInfo.Sequence = null;
                }

                return warnInfo;
            }
            #endregion

            #region SequenceGroup
            else if(parent is ISequenceGroup)
            {
                variable = ((ISequenceGroup)parent).Variables.FirstOrDefault(item => item.Name.Equals(name));
                if (variable == null)
                {
                    throw new TestflowDataException(ModuleErrorCode.VariableNotFound, $"Variable {name} not found in SequenceGroup {parent.Name}");
                }
                IWarningInfo warnInfo = CheckVariableValue(variable);
                if (warnInfo != null)
                {
                    warnInfo.SequenceGroup = (ISequenceGroup)parent;
                    warnInfo.Sequence = null;
                }
                return warnInfo;
            }
            #endregion

            #region Sequence
            else if(parent is ISequence)
            {
                variable = ((ISequence)parent).Variables.FirstOrDefault(item => item.Name.Equals(name));
                if (variable == null)
                {
                    throw new TestflowDataException(ModuleErrorCode.VariableNotFound, $"Variable {name} not found in Sequence {parent.Name}");
                }
                IWarningInfo warnInfo = CheckVariableValue(variable);
                if (warnInfo != null)
                {
                    warnInfo.SequenceGroup = (ISequenceGroup)((ISequence)parent).Parent;
                    warnInfo.Sequence = (ISequence)parent;
                }
                return warnInfo;
            }
            #endregion

            else
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidParent, "Parent must be ITestProject, ISequenceGroup, or ISequence");
            }

        }

        //todo I18n
        private IWarningInfo CheckVariableValue(IVariable variable)
        {
            try
            {
                if(ValueConvertor.CheckValue(variable.Type.Name, variable.Value))
                {
                    return null;
                }
                else
                {
                    return new WarningInfo()
                    {
                        WarnCode = WarnCode.VariableValueInvalid,
                        Infomation = $"Variable {variable.Name} value: \"{variable.Value}\" failed to parse into type of {variable.Type.Name}. Will cause issues during runtime.",
                        SequenceStep = null
                    };
                }
            }
            catch (Exception ex)
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidType, "Please make sure VariableType is Value and Type is of the following: Int32, boolean, string, double, e.t.c." + ex.Message);
            }
        }
    }
    #endregion
}
