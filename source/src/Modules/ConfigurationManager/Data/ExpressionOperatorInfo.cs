using System;
using System.Xml;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.Data.Expression;

namespace Testflow.ConfigurationManager.Data
{
    /// <summary>
    /// 表达式操作符配置项
    /// </summary>
    public class ExpressionOperatorInfo : IExpressionOperatorInfo
    {
        /// <summary>
        /// 操作符名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 操作符描述信息
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 操作符样式
        /// </summary>
        [XmlElement(Order = 1)]
        public string Symbol { get; set; }

        /// <summary>
        /// 操作符格式化字符串，0对应Source，1对应Target
        /// </summary>
        [XmlElement(Order = 2)]
        public string FormatString { get; set; }

        /// <summary>
        /// 运算符计算所在的程序集
        /// </summary>
        [XmlElement(Order = 3)]
        public string Assembly { get; set; }

        /// <summary>
        /// 运算法的计算类，该类必须继承自Testflow.Usr.Expression.IExpressionFunction
        /// </summary>
        [XmlElement(Order = 4)]
        public string ClassName { get; set; }

        /// <summary>
        /// 运算符计算所在的程序集
        /// </summary>
        [XmlElement(Order = 5)]
        public string SourceAssembly { get; set; }

        /// <summary>
        /// 运算法的计算类，该类必须继承自Testflow.Usr.Expression.IExpressionFunction
        /// </summary>
        [XmlElement(Order = 6)]
        public string SourceClassName { get; set; }

        /// <summary>
        /// 源数据类型
        /// </summary>
        [XmlIgnore]
        public ITypeData SourceClassType { get; set; }

        /// <summary>
        /// 计算的方法
        /// </summary>
        [XmlIgnore]
        public IExpressionCalculator CalculationClass { get; set; }
    }
}