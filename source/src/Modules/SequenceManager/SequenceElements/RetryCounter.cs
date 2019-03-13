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
            this.MaxRetryTimes = 0;
            this.RetryEnabled = true;
            this.CounterVariable = string.Empty;
        }

        public string Name { get; set; }
        public int MaxRetryTimes { get; set; }
        public bool RetryEnabled { get; set; }
        public string CounterVariable { get; set; }

        public IRetryCounter Clone()
        {
            RetryCounter loopCounter = new RetryCounter()
            {
                Name = this.Name + Constants.CopyPostfix,
                MaxRetryTimes = this.MaxRetryTimes,
                RetryEnabled = this.RetryEnabled,
                CounterVariable = string.Empty
            };
            return loopCounter;
        }

        #region 序列化声明及反序列化构造

        public RetryCounter(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillSerializationInfo(info, this);
        }

        #endregion
    }
}