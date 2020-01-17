using System.Collections.Generic;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式符号集合
    /// </summary>
    public interface IExpressionOperatorCollection : IList<IExpressionOperatorInfo>
    {
        /// <summary>
        /// 根据计算符符号获取运算符信息
        /// </summary>
        IExpressionOperatorInfo GetOperatorInfo(string operatorToken);

        /// <summary>
        /// 根据计算符名称获取运算符信息
        /// </summary>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        IExpressionOperatorInfo GetOperatorInfoByName(string operatorName);
    }
}