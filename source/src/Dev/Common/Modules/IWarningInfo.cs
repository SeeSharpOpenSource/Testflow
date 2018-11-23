using Testflow.Data.Sequence;

namespace Testflow.Modules
{
    /// <summary>
    /// 警告信息
    /// </summary>
    public interface IWarningInfo
    {
        /// <summary>
        /// 警告所在的序列组
        /// </summary>
        ISequenceGroup SequenceGroup { get; set; }

        /// <summary>
        /// 警告所在的序列
        /// </summary>
        ISequence Sequence { get; set; }

        /// <summary>
        /// 警告所在的序列Step
        /// </summary>
        ISequenceStep SequenceStep { get; set; }

        /// <summary>
        /// 告警信息
        /// </summary>
        string WarningInfo { get; set; }
    }
}