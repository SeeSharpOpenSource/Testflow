﻿using System;
using System.Xml.Serialization;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列参数配置相关信息
    /// </summary>
    public interface ISequenceParameterInfo : ISequenceElement
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
        /// 序列参数创建时间
        /// </summary>
        DateTime CreateTime { get; set; }

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
