using System;
using System.Xml.Serialization;

namespace Testflow.DataInterface.Sequence
{
    public interface ISequenceParameterInfo
    {
        /// <summary>
        /// 序列参数的格式版本
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// 对应测试序列组的哈希值
        /// </summary>
        string Hash { get; set; }

        /// <summary>
        /// 对应测试序列组当前的MD5
        /// </summary>
        string MD5 { get; set; }
        
        /// <summary>
        /// 序列参数最新的更新时间
        /// </summary>
        DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 序列参数文件路径
        /// </summary>
        [XmlIgnore]
        string Path { get; set; }
        
        /// <summary>
        /// 测试序列组是否被修改的标识位
        /// </summary>
        bool Modified { get; set; }
    }
}
