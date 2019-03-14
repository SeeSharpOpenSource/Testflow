using System;
using System.Runtime.Serialization;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class LoopCounter : ILoopCounter
    {
        public LoopCounter()
        {
            this.Name = string.Empty;
            this.MaxValue = 0;
            this.CounterEnabled = true;
            this.CounterVariable = string.Empty;
        }

        public string Name { get; set; }
        public int MaxValue { get; set; }
        public bool CounterEnabled { get; set; }
        public string CounterVariable { get; set; }

        public ILoopCounter Clone()
        {
            LoopCounter loopCounter = new LoopCounter()
            {
                Name = this.Name + Constants.CopyPostfix,
                MaxValue = this.MaxValue,
                CounterEnabled = this.CounterEnabled,
                CounterVariable = string.Empty
            };
            return loopCounter;
        }

        #region 序列化声明及反序列化构造

        public LoopCounter(SerializationInfo info, StreamingContext context)
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