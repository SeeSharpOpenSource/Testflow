using Testflow.Modules;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式函数接口
    /// </summary>
    public interface IExpressionCalculator
    {
        /// <summary>
        /// 表达式描述符
        /// </summary>
        string Operator { get; }

        /// <summary>
        /// 判断待测试的数据类型是否匹配
        /// </summary>
        bool IsValidValue(out string errorInfo, object sourceValue, params object[] arguments);

        /// <summary>
        /// 计算表达式结果
        /// </summary>
        object Calculate(object sourceValue, params object[] arguments);
    }
}