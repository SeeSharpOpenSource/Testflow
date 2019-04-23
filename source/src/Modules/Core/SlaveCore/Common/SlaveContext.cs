using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Log;
using Testflow.Runtime;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner;
using Testflow.SlaveCore.Runner.Model;
using Testflow.SlaveCore.SlaveFlowControl;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore.Common
{
    internal class SlaveContext : IDisposable
    {
        private readonly Dictionary<string, string> _configData;
        private readonly Dictionary<string, Func<string, object>> _valueConvertor;

        public SlaveContext(string configDataStr)
        {
            _configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configDataStr);
            this._valueConvertor = new Dictionary<string, Func<string, object>>(10);
            _valueConvertor.Add(typeof(string).Name, strValue => strValue);
            _valueConvertor.Add(typeof(long).Name, strValue => long.Parse(strValue));
            _valueConvertor.Add(typeof(int).Name, strValue => int.Parse(strValue));
            _valueConvertor.Add(typeof(uint).Name, strValue => uint.Parse(strValue));
            _valueConvertor.Add(typeof(short).Name, strValue => short.Parse(strValue));
            _valueConvertor.Add(typeof(ushort).Name, strValue => ushort.Parse(strValue));
            _valueConvertor.Add(typeof(char).Name, strValue => char.Parse(strValue));
            _valueConvertor.Add(typeof(byte).Name, strValue => byte.Parse(strValue));

            SessionId = this.GetProperty<int>("Session");
            State = RuntimeState.NotAvailable;
            this.StatusQueue = new LocalEventQueue<SequenceStatusInfo>(CoreConstants.DefaultEventsQueueSize);
            this.LogSession = TODO;
            this.I18N = I18N.GetInstance(Constants.I18nName);
            this.MessageTransceiver = new MessageTransceiver(this, SessionId);
            this.UplinkMsgPacker = new StatusMonitor(this);
        }

        public I18N I18N { get; }

        public int SessionId { get; }

        public ILogSession LogSession { get; }

        public MessageTransceiver MessageTransceiver { get; }

        public SlaveController Controller { get; set; }

        public TestRunner Runner { get; set; }

        public RunnerType SequenceType { get; set; }

        public ISequenceFlowContainer Sequence { get; set; }

        public ExecutionModel ExecutionModel { get; set; }

        public VariableMapper VariableMapper { get; set; }

        public SessionTaskEntity SessionTaskEntity { get; set; }

        public AssemblyInvoker TypeInvoker { get; set; }

        public LocalEventQueue<SequenceStatusInfo> StatusQueue { get; }

        public StatusMonitor UplinkMsgPacker { get; }

        private int _runtimeState;

        /// <summary>
        /// 全局状态。配置规则：哪里最早获知全局状态变更就在哪里更新。
        /// </summary>
        public RuntimeState State
        {
            get { return (RuntimeState) _runtimeState; }
            set
            {
                // 如果当前状态大于等于待更新状态则不执行。因为在一次运行的实例中，状态的迁移是单向的。
                if ((int) value <= _runtimeState)
                {
                    return;
                }
                Thread.VolatileWrite(ref _runtimeState, (int) value);
            }
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            Type dataType = typeof(TDataType);
            if (!_configData.ContainsKey(propertyName))
            {
                throw new ArgumentException($"unexist property {propertyName}");
            }
            if (!_configData.ContainsKey(dataType.Name) && !dataType.IsEnum)
            {
                throw new InvalidCastException($"Unsupported cast type: {dataType.Name}");
            }
            object value;
            if (dataType.IsEnum)
            {
                value = Enum.Parse(dataType, _configData[propertyName]);
            }
            else
            {
                value = _valueConvertor[dataType.Name].Invoke(_configData[propertyName]);
            }
            return (TDataType) value;
        }

        public void Dispose()
        {
            Runner?.Dispose();
        }
    }
}