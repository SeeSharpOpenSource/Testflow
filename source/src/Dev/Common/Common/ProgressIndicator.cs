using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Testflow.Common
{
    /// <summary>
    /// 进度标识器
    /// </summary>
    [Serializable]
    public class ProgressIndicator
    {
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue => 100;

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue => 0;

        private double _value;
        /// <summary>
        /// 当前的进度值
        /// </summary>
        public double Value
        {
            get
            {
                Thread.VolatileRead(ref _value);
                return _value;
            }
            set { this._value = value; }
        }

        /// <summary>
        /// 是否开始
        /// </summary>
        public bool IsStart => Math.Abs(MinValue - _value) > Constants.MinDoubleValue;

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool IsEnd => Math.Abs(MaxValue - _value) > Constants.MinDoubleValue;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ProgressIndicator()
        {
            this._value = 0;
        }

        /// <summary>
        /// 复位
        /// </summary>
        public void Reset()
        {
            this._value = 0;
        }
    }
}