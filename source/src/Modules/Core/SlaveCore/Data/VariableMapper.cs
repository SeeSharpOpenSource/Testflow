using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Testflow.CoreCommon;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore.Data
{
    internal class VariableMapper : IDisposable
    {
        private readonly Dictionary<string, object> _variables;
        private readonly SlaveContext _context;
        // 关键变量集合操作锁
        private SpinLock _keyVarLock;
        // 同步变量读写锁
        private ReaderWriterLockSlim _syncVarLock;
        private readonly HashSet<string> _keyVariables;
        private readonly HashSet<string> _syncVariables;
        private readonly List<string> _fullTraceVariables;

        public VariableMapper(SlaveContext context)
        {
            ISequenceFlowContainer sequenceData = context.Sequence;
            this._variables = new Dictionary<string, object>(512);
            this._keyVariables = new HashSet<string>();
            this._syncVariables = new HashSet<string>();
            this._context = context;
            _fullTraceVariables = new List<string>(Constants.DefaultRuntimeSize);
            if (context.SequenceType == RunnerType.TestProject)
            {
                ITestProject testProject = (ITestProject)sequenceData;
                AddVariables(testProject.Variables, false);
                AddVariables(testProject.SetUp.Variables, false);
                AddVariables(testProject.TearDown.Variables, false);
            }
            else
            {
                bool addSessionVarToSyncSet = ExecutionModel.ParallelExecution == context.ExecutionModel;
                ISequenceGroup sequenceGroup = (ISequenceGroup)sequenceData;
                AddVariables(sequenceGroup.Variables, addSessionVarToSyncSet);
                AddVariables(sequenceGroup.SetUp.Variables, false);
                AddVariables(sequenceGroup.TearDown.Variables, false);
                foreach (ISequence sequence in sequenceGroup.Sequences)
                {
                    AddVariables(sequence.Variables, false);
                }
            }
            this._keyVarLock = new SpinLock(false);
            this._syncVarLock = new ReaderWriterLockSlim();
        }

        private void AddVariables(IVariableCollection variables, bool addToSyncSet)
        {
            int sessionId = _context.SessionId;
            foreach (IVariable variable in variables)
            {
                string variableName = CoreUtils.GetRuntimeVariableName(sessionId, variable);
                object value = null;
                // 如果变量已匹配类型并且配置的值有效，则初始化变量值。如果是值类型但是未配置值则获取默认值
                if (variable.VariableType != VariableType.Undefined && null != variable.Type)
                {
                    value = CoreUtils.IsValidVaraibleValue(variable)
                        ? _context.TypeInvoker.CastConstantValue(variable.Type, variable.Value)
                        : _context.Convertor.GetDefaultValue(variable.Type);
                }
                this._variables.Add(variableName, value);
                // 如果变量的OI报告级别配置为trace则添加变量到监控数据中
                // 如果变量OI报告级别为最终结果或者报告级别不为None，则添加变量到返回数据
                if (variable.OIRecordLevel == RecordLevel.FullTrace || variable.ReportRecordLevel == RecordLevel.FullTrace)
                {
                    _fullTraceVariables.Add(variableName);
                    _context.ReturnVariables.Add(variableName);
                }
                else if (variable.OIRecordLevel == RecordLevel.Trace || variable.ReportRecordLevel == RecordLevel.Trace)
                {
                    _context.TraceVariables.Add(variableName);
                    _context.ReturnVariables.Add(variableName);
                }
                else if (variable.OIRecordLevel == RecordLevel.FinalResult ||
                         variable.ReportRecordLevel != RecordLevel.None)
                {
                    _context.ReturnVariables.Add(variableName);
                }
                if (addToSyncSet)
                {
                    _syncVariables.Add(variableName);
                }
            }
        }

        public void SetParamValue(string variableName, string paramValue, object value)
        {
            if (_syncVariables.Contains(variableName))
            {
                _syncVarLock.EnterWriteLock();
                this._variables[variableName] = SetParamValue(paramValue, _variables[variableName], value);
                _syncVarLock.ExitWriteLock();
            }
            else
            {
                this._variables[variableName] = SetParamValue(paramValue, _variables[variableName], value);
            }

            // 监视变量值如果被更新，则添加到值更新列表中，在状态上报时上传该值
            if (_context.TraceVariables.Contains(variableName))
            {
                bool getlock = false;
                _keyVarLock.Enter(ref getlock);
                _keyVariables.Add(variableName);
                _keyVarLock.Exit();
            }
        }

        private object SetParamValue(string paramValueStr, object varValue, object paramValue)
        {
            if (!paramValueStr.Contains(Constants.PropertyDelim))
            {
                return paramValue;
            }
            object parentValue = varValue;
            Type parentType = varValue.GetType();
            string[] paramElems = paramValueStr.Split(Constants.PropertyDelim.ToCharArray());
            BindingFlags binding = BindingFlags.Public | BindingFlags.Instance;
            for (int i = 1; i < paramElems.Length - 1; i++)
            {
                PropertyInfo propertyInfo = parentType.GetProperty(paramElems[i], binding);
                if (null == propertyInfo)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetFStr("UnexistVariable", paramValueStr));
                }
                parentType = propertyInfo.PropertyType;
                parentValue = propertyInfo.GetValue(parentValue);
            }
            PropertyInfo paramProperty = parentType.GetProperty(paramElems[paramElems.Length - 1], binding);
            if (null == paramProperty)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetFStr("UnexistVariable", paramValueStr));
            }
            // 如果变量值不为null，并且变量类型和待写入属性类型不匹配则执行类型转换
            Type propertyType = paramProperty.PropertyType;
            if (null != paramValue && !propertyType.IsInstanceOfType(paramValue) &&
                _context.Convertor.IsValidCast(paramValue.GetType(), propertyType))
            {
                paramValue = _context.Convertor.CastValue(propertyType, paramValue);
            }
            paramProperty.SetValue(parentValue, paramValue);
            return varValue;
        }

        // 清空序列的变量
        public void ClearSequenceVariables(ISequenceFlowContainer sequence)
        {
            string nameRegex = CoreUtils.GetExactVariableNameRegex(sequence, _context.SessionId);
            Regex varRegex = new Regex(nameRegex);
            List<string> variableNames = new List<string>(_variables.Keys);
            foreach (string variableName in variableNames)
            {
                if (varRegex.IsMatch(variableName))
                {
                    // TODO 只将该值配置为null，不会删除变量定义
                    _variables[variableName] = null;
                }
            }
        }

        public object GetParamValue(string variableName, string paramValueStr, ITypeData targetType)
        {
            object value;
            if (_syncVariables.Contains(variableName))
            {
                _syncVarLock.EnterReadLock();
                value = GetParamValue(paramValueStr, this._variables[variableName], targetType);
                _syncVarLock.ExitReadLock();
            }
            else
            {
                value = GetParamValue(paramValueStr, this._variables[variableName], targetType);
            }
            return value;
        }

        private object GetParamValue(string paramValueStr, object varValue, ITypeData targetType)
        {
            object paramValue = GetParamValue(varValue, paramValueStr);

            Type dstType = _context.TypeInvoker.GetType(targetType);
            // 如果Value不为null，并且value和targetType不匹配，则执行转换
            if (null != paramValue && !dstType.IsInstanceOfType(paramValue) &&
                _context.Convertor.IsValidCast(paramValue.GetType(), dstType))
            {
                paramValue = _context.Convertor.CastValue(targetType, paramValue);
            }
            return paramValue;
        }

        private static object GetParamValue(object varValue, string paramValueStr)
        {
            object paramValue = varValue;
            // 如果ParamValue使用了类属性的定义，则需要按层取出真实属性的值
            if (paramValueStr.Contains(Constants.PropertyDelim))
            {
                string[] paramElems = paramValueStr.Split(Constants.PropertyDelim.ToCharArray());
                BindingFlags binding = BindingFlags.Public | BindingFlags.Instance;
                for (int i = 1; i < paramElems.Length; i++)
                {
                    PropertyInfo propertyInfo = paramValue.GetType().GetProperty(paramElems[i], binding);
                    if (null == propertyInfo)
                    {
                        I18N i18N = I18N.GetInstance(Constants.I18nName);
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                            i18N.GetFStr("UnexistVariable", paramValueStr));
                    }
                    paramValue = propertyInfo.GetValue(paramValue);
                }
            }
            return paramValue;
        }

        public Dictionary<string, string> GetKeyVariablesValues(ISequenceFlowContainer sequence)
        {
            bool getlock = false;
            _keyVarLock.Enter(ref getlock);

            if ((_keyVariables.Count == 0 || null == sequence) && _fullTraceVariables.Count == 0)
            {
                _keyVarLock.Exit();
                return null;
            }
            // 获取值已经变更的对象列表拷贝，并清空上个报告周期的值变更列表
            List<string> keyVarNames = null;
            
            switch (_context.ExecutionModel)
            {
                case ExecutionModel.SequentialExecution:
                    // 如果序列串行执行，则直接清空所有关键变量，直接返回值
                    keyVarNames = new List<string>(_keyVariables);
                    keyVarNames.AddRange(_fullTraceVariables);
                    _keyVariables.Clear();
                    break;
                case ExecutionModel.ParallelExecution:
                    // 如果序列并行执行，则只获取当前Sequence的值列表
                    keyVarNames = new List<string>(_keyVariables.Count);
                    string varNameRegex = CoreUtils.GetVariableNameRegex(sequence, _context.SessionId);
                    Regex regex = new Regex(varNameRegex);
                    keyVarNames.AddRange(_keyVariables.Where(keyVariable => regex.IsMatch(keyVariable)));
                    foreach (string keyVarName in keyVarNames)
                    {
                        _keyVariables.Remove(keyVarName);
                    }
                    keyVarNames.AddRange(_fullTraceVariables);
                    break;
                default:
                    _keyVarLock.Exit();
                    throw new ArgumentOutOfRangeException();
            }
            _keyVarLock.Exit();

            Dictionary<string, string> watchData = GetKeyVariableValues(keyVarNames);
            return watchData;
        }

        public string GetWatchDataValue(string variable, string watchStr)
        {
            if (!_variables.ContainsKey(variable))
            {
                return Constants.IllegalValue;
            }
            object paramValue = GetParamValue(_variables[variable], watchStr);
            string paramStr;
            GetVariableStringValue(paramValue, out paramStr);
            return paramStr;
        }

        private Dictionary<string, string> GetKeyVariableValues(List<string> keyVarNames)
        {
            Dictionary<string, string> watchDataValues = new Dictionary<string, string>(keyVarNames.Count);
            int index = 0;
            string varName = "";
            string varValueStr;
            while (index < keyVarNames.Count)
            {
                try
                {
                    while (index < keyVarNames.Count)
                    {
                        varName = keyVarNames[index++];
                        object varValue = _variables[varName];
                        GetVariableStringValue(varValue, out varValueStr);
                        watchDataValues.Add(varName, varValueStr);
                    }
                }
                catch (Exception ex)
                {
                    _context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformSession, ex, $"Deserialize variable '{varName}' error.");
                    watchDataValues.Add(varName, CoreConstants.ErrorVarValue);
                }
            }

            return watchDataValues;
        }

        public Dictionary<string, string> GetReturnDataValues()
        {
            Dictionary<string, string> returnDataValues = new Dictionary<string, string>(_keyVariables.Count);

            if (_context.ReturnVariables.Count == 0)
            {
                return returnDataValues;
            }

            string varName = "";
            HashSet<string>.Enumerator returnEnumerator = _context.ReturnVariables.GetEnumerator();
            bool hasNext = returnEnumerator.MoveNext();
            string varValueStr;
            while (hasNext)
            {
                try
                {
                    while (hasNext)
                    {
                        varName = returnEnumerator.Current;
                        hasNext = returnEnumerator.MoveNext();
                        object varValue = _variables[varName];
                        GetVariableStringValue(varValue, out varValueStr);
                        returnDataValues.Add(varName, varValueStr);
                    }
                }
                catch (Exception ex)
                {
                    _context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformSession, ex, $"Deserialize variable '{varName}' error.");
                    returnDataValues.Add(varName, CoreConstants.ErrorVarValue);
                }
            }
            return returnDataValues;
        }

        public Dictionary<string, string> GetReturnDataValues(ISequenceFlowContainer sequence)
        {
            Dictionary<string, string> returnDataValues = new Dictionary<string, string>(_context.ReturnVariables.Count);

            if (_context.ReturnVariables.Count == 0)
            {
                return returnDataValues;
            }

            string nameRegex = CoreUtils.GetVariableNameRegex(sequence, _context.SessionId);
            Regex regex = new Regex(nameRegex);

            string varName = "";
            HashSet<string>.Enumerator returnEnumerator = _context.ReturnVariables.GetEnumerator();
            bool hasNext = returnEnumerator.MoveNext();
            string varValueStr;
            while (hasNext)
            {
                try
                {
                    while (hasNext)
                    {
                        varName = returnEnumerator.Current;
                        hasNext = returnEnumerator.MoveNext();
                        // 如果不匹配序列，则跳过
                        if (!regex.IsMatch(varName))
                        {
                            continue;
                        }
                        object varValue = _variables[varName];
                        GetVariableStringValue(varValue, out varValueStr);
                        returnDataValues.Add(varName, varValueStr);
                    }
                }
                catch (Exception ex)
                {
                    _context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformSession, ex, $"Deserialize variable '{varName}' error.");
                    returnDataValues.Add(varName, CoreConstants.ErrorVarValue);
                }
            }

            bool getLock = false;
            _keyVarLock.Enter(ref getLock);
            foreach (string returnVar in returnDataValues.Keys)
            {
                _context.ReturnVariables.Remove(returnVar);
            }
            _keyVarLock.Exit();

            return returnDataValues;
        }

        private void GetVariableStringValue(object varValue, out string value)
        {
            if (null == varValue)
            {
                value =  CoreConstants.NullValue;
                return;
            }
            Type varType = varValue.GetType();
            // 简单数据类型
            Type stringType = typeof(string);
            if (varType == stringType)
            {
                value = (string)varValue;
            }
            else if (_context.Convertor.IsValidCast(varType, stringType))
            {
                value = (string)_context.Convertor.CastValue(stringType, varValue);
            }
            // 枚举类型
            else if (varType.IsEnum)
            {
                value = varValue.ToString();
            }
            // 类类型
            else if (varType.IsClass)
            {
                value = JsonConvert.SerializeObject(varValue);
            }
            // 非简单类型的值类型(结构体)
            else if (varType.IsValueType)
            {
                value = JsonConvert.SerializeObject(varValue);
            }
            else
            {
                value = string.Empty;
            }
        }

        public void Dispose()
        {
            foreach (object value in _variables.Values)
            {
                (value as IDisposable)?.Dispose();
            }
            _variables.Clear();
            _syncVarLock.Dispose();
        }
    }
}