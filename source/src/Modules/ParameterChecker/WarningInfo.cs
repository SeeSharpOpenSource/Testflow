using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Modules;

namespace Testflow.ParameterChecker
{
    public class WarningInfo : IWarningInfo
    {
        /// <summary>
        /// 警告所在的序列组
        /// </summary>
        public ISequenceGroup SequenceGroup { get; set; }

        /// <summary>
        /// 警告所在的序列
        /// </summary>
        public ISequence Sequence { get; set; }

        /// <summary>
        /// 警告所在的序列Step
        /// </summary>
        public ISequenceStep SequenceStep { get; set; }

        /// <summary>
        /// 告警信息
        /// </summary>
        public string Infomation { get; set; }

        /// <summary>
        /// 告警码
        /// </summary>
        public int WarnCode { get; set; }
    }
}
