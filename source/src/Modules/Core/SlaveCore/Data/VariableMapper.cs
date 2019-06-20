using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Data
{
    internal class VariableMapper : IDisposable
    {
        private readonly Dictionary<string, object> _variables;
        private readonly SlaveContext _context;

        private SpinLock _keyVarLock;
        private HashSet<string> _keyVariables;

        public VariableMapper(SlaveContext context)
        {
            ISequenceFlowContainer sequenceData = context.Sequence;
            this._variables = new Dictionary<string, object>(512);
            this._keyVariables = new HashSet<string>();
            this._context = context;
            if (context.SequenceType == RunnerType.TestProject)
            {
                ITestProject testProject = (ITestProject)sequenceData;
                AddVariables(testProject.Variables);
                AddVariables(testProject.SetUp.Variables);
                AddVariables(testProject.TearDown.Variables);
            }
            else
            {
                ISequenceGroup sequenceGroup = (ISequenceGroup)sequenceData;
                AddVariables(sequenceGroup.Variables);
                AddVariables(sequenceGroup.SetUp.Variables);
                AddVariables(sequenceGroup.TearDown.Variables);
                foreach (ISequence sequence in sequenceGroup.Sequences)
                {
                    AddVariables(sequence.Variables);
                }
            }
            this._keyVarLock = new SpinLock();
        }

        private void AddVariables(IVariableCollection variables)
        {
            int sessionId = _context.SessionId;
            foreach (IVariable variable in variables)
            {
                string variableName = CoreUtils.GetRuntimeVariableName(sessionId, variable);
                this._variables.Add(variableName, null);
                // 如果变量的OI报告级别配置为trace则添加变量到监控数据中
                // 如果变量OI报告级别为最终结果或者报告级别不为None，则添加变量到返回数据
                if (variable.OIRecordLevel == RecordLevel.Trace)
                {
                    _context.WatchDatas.Add(variableName);
                    _context.ReturnDatas.Add(variableName);
                }
                else if (variable.OIRecordLevel == RecordLevel.FinalResult ||
                         variable.ReportRecordLevel != RecordLevel.None)
                {
                    _context.ReturnDatas.Add(variableName);
                }
            }
        }

        public void SetParamValue(string variableName, string paramValue, object value, bool recordStatus)
        {
            this._variables[variableName] = ModuleUtils.SetParamValue(paramValue, _variables[variableName], value);

            // 监视变量值如果被更新，则添加到值更新列表中，在状态上报时上传该值
            if (recordStatus && _context.WatchDatas.Contains(variableName))
            {
                bool getlock = false;
                _keyVarLock.Enter(ref getlock);
                _keyVariables.Add(variableName);
                _keyVarLock.Exit();
            }
        }

        public object GetParamValue(string variableName, string paramValueStr)
        {
            return ModuleUtils.GetParamValue(paramValueStr, this._variables[variableName]);
        }

        public Dictionary<string, string> GetWatchDataValues()
        {
            Dictionary<string, string> watchDataValues = new Dictionary<string, string>(_keyVariables.Count);

            if (_keyVariables.Count == 0)
            {
                return watchDataValues;
            }

            // 获取值已经变更的对象列表拷贝，并清空上个报告周期的值变更列表
            bool getlock = false;
            _keyVarLock.Enter(ref getlock);
            List<string> keyVarNames = new List<string>(_keyVariables);
            _keyVariables.Clear();
            _keyVarLock.Exit();

            int index = 0;
            string varName = "";
            while (index < keyVarNames.Count)
            {
                try
                {
                    while (index < keyVarNames.Count)
                    {
                        varName = keyVarNames[index++];
                        object varValue = _variables[varName];
                        if (null == varValue)
                        {
                            watchDataValues.Add(varName, CoreConstants.NullValue);
                            continue;
                        }
                        Type varType = varValue.GetType();
                        if (varType.IsValueType || varType.IsEnum)
                        {
                            watchDataValues.Add(varName, varValue.ToString());
                        }
                        else if (varType == typeof(string))
                        {
                            watchDataValues.Add(varName, (string) varValue);
                        }
                        else if (varType.IsClass)
                        {
                            string varValueString = JsonConvert.SerializeObject(varValue);
                            watchDataValues.Add(varName, (string)varValueString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformSession, ex,
                        $"Deserialize variable '{varName}' error.");
                    watchDataValues.Add(varName, CoreConstants.ErrorVarValue);
                }
            }

            return watchDataValues;
        }

        public Dictionary<string, string> GetReturnDataValues()
        {
            Dictionary<string, string> returnDataValues = new Dictionary<string, string>(_keyVariables.Count);

            if (_context.ReturnDatas.Count == 0)
            {
                return returnDataValues;
            }

            string varName = "";
            HashSet<string>.Enumerator returnEnumerator = _context.ReturnDatas.GetEnumerator();
            bool hasNext = returnEnumerator.MoveNext();
            while (hasNext)
            {
                try
                {
                    while (hasNext)
                    {
                        varName = returnEnumerator.Current;
                        hasNext = returnEnumerator.MoveNext();
                        object varValue = _variables[varName];
                        if (null == varValue)
                        {
                            returnDataValues.Add(varName, CoreConstants.NullValue);
                            continue;
                        }
                        Type varType = varValue.GetType();
                        if (varType.IsValueType || varType.IsEnum)
                        {
                            returnDataValues.Add(varName, varValue.ToString());
                        }
                        else if (varType == typeof(string))
                        {
                            returnDataValues.Add(varName, (string)varValue);
                        }
                        else if (varType.IsClass)
                        {
                            string varValueString = JsonConvert.SerializeObject(varValue);
                            returnDataValues.Add(varName, (string)varValueString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformSession, ex,
                        $"Deserialize variable '{varName}' error.");
                    returnDataValues.Add(varName, CoreConstants.ErrorVarValue);
                }
            }
            return returnDataValues;
        }

        public void Dispose()
        {
            foreach (object value in _variables.Values)
            {
                (value as IDisposable)?.Dispose();
            }
            _variables.Clear();
        }
    }
}