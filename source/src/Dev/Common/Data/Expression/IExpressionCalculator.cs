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
        IExpressionOperatorInfo OperationInfo { get; }

        /// <summary>
        /// 获取表达式的显示方式
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="target">目标数据</param>
        /// <returns></returns>
        string GetExpressionString(string source, string target);

        /// <summary>
        /// 判断source的值是否合法
        /// </summary>
        bool IsValidSource(IComInterfaceManager interfaceManager, ITypeData sourceType);

        /// <summary>
        /// 判断source的值是否合法
        /// </summary>
        bool IsValidSource(IComInterfaceManager interfaceManager, object sourceValue);


        /// <summary>
        /// 计算表达式结果
        /// </summary>
        object Calculate(object sourceValue, object targetValue);
    }
}