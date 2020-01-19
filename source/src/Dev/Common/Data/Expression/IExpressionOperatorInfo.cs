using System;
using System.Xml.Serialization;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式操作符配置项
    /// </summary>
    public interface IExpressionOperatorInfo
    {
        /// <summary>
        /// 操作符名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 操作符描述信息
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 操作符样式
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// 操作符格式化字符串，0对应Source，1对应Target
        /// </summary>
        string FormatString { get; }

        /// <summary>
        /// 运算符计算所在的程序集
        /// </summary>
        string Assembly { get; set; }

        /// <summary>
        /// 运算法的计算类，该类必须继承自Testflow.Usr.Expression.IExpressionFunction
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// 运算符计算所在的程序集
        /// </summary>
        string SourceAssembly { get; set; }

        /// <summary>
        /// 目标数据类型名称
        /// </summary>
        string SourceClassName { get; set; }

        /// <summary>
        /// 源数据类型
        /// </summary>
        ITypeData SourceClassType { get; set; }

        /// <summary>
        /// 计算的方法
        /// </summary>
        IExpressionCalculator CalculationClass { get; }
    }
}