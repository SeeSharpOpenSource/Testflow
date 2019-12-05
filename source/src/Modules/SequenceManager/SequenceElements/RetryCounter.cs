using System;
using System.Runtime.Serialization;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class RetryCounter : IRetryCounter
    {
        public RetryCounter()
        {
            this.Name = string.Empty;
            this._maxRetryTimes = 2;
            this.PassTimes = 1;
            this.RetryEnabled = true;
            this.CounterVariable = string.Empty;
            this.PassCountVariable = string.Empty;
        }

        public string Name { get; set; }

        private int _maxRetryTimes;

        public int MaxRetryTimes
        {
            get { return _maxRetryTimes; }
            set { _maxRetryTimes = value < 2 ? 2 : value; }
        }

        private int _passTimes;

        public int PassTimes
        {
            get { return _passTimes; }
            set { _passTimes = value < 1 ? 1 : value; }
        }

        public bool RetryEnabled { get; set; }
        public string CounterVariable { get; set; }
        public string PassCountVariable { get; set; }

        public IRetryCounter Clone()
        {
            RetryCounter retryCounter = new RetryCounter()
            {
                Name = this.Name + Constants.CopyPostfix,
                MaxRetryTimes = this.MaxRetryTimes,
                RetryEnabled = this.RetryEnabled,
                CounterVariable = this.CounterVariable,
                PassTimes = this.PassTimes,
                PassCountVariable = this.PassCountVariable,
            };
            return retryCounter;
        }

        #region 序列化声明及反序列化构造

        public RetryCounter(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillSerializationInfo(info, this);
        }

        #endregion
    }
}