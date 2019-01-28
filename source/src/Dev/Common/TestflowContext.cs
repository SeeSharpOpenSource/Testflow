﻿using System.Threading;

namespace Testflow
{
    /// <summary>
    /// Testflow平台的上下文信息
    /// </summary>
    public class TestflowContext
    {
        private int _stateValue;
        /// <summary>
        /// 平台当前状态
        /// </summary>
        public TestflowStates State
        {
            get
            {
                Thread.VolatileRead(ref _stateValue);
                return (TestflowStates) _stateValue;
            }
            set { this._stateValue = (int) value; }
        }

        internal TestflowContext()
        {
            this._stateValue = (int) TestflowStates.Unavailable;
        }
    }
}