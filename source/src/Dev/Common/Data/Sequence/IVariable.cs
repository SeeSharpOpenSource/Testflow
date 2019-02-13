using System;
using System.Xml.Serialization;
using Testflow.Data.Description;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 变量数据
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 变量的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 变量的类型
        /// </summary>
        VariableType VariableType { get; set; }

        /// <summary>
        /// 变量的描述信息
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 在日志中的记录级别
        /// </summary>
        RecordLevel LogRecordLevel { get; set; }

        /// <summary>
        /// 在报表中的记录级别
        /// </summary>
        RecordLevel ReportRecordLevel { get; set; }

        /// <summary>
        /// 在操作面板中的记录级别
        /// </summary>
        RecordLevel OIRecordLevel { get; set; }

        /// <summary>
        /// 变量的值，如果没有则为null
        /// </summary>
        [XmlIgnore]
        string Value { get; set; }

        /// <summary>
        /// 变量的作用域范围
        /// </summary>
        [XmlIgnore]
        ISequenceFlowContainer Parent { get; set; }
    }
}