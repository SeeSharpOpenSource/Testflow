using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Data.Expression;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.Expression
{
    internal class OperatorAdapter
    {
        /// <summary>
        /// 运算的优先级
        /// </summary>
        public int Priority { get; }

        // 保存表达式中占位符的位置到参数索引的映射
        private readonly Dictionary<int, int> _orderToIndexMapping;

        // 表达式的匹配模式
        private readonly Regex _expressionRegex;

        public OperatorAdapter(ExpressionOperatorInfo operatorInfo, string argumentPattern, HashSet<char> metaCharacters)
        {
            const string argName = "ArgOfExp";
            StringBuilder formatCache = new StringBuilder(operatorInfo.FormatString, 200);
            this.Priority = operatorInfo.Priority;
            Regex argsRegex = new Regex("\\{([0-9]+)\\}");
            MatchCollection matches = argsRegex.Matches(operatorInfo.FormatString);
            _orderToIndexMapping = new Dictionary<int, int>(matches.Count);
            int orderIndex = 0;
            // 缓存表达式中参数顺序到参数编号的映射，0为Source
            foreach (Match matchData in matches)
            {
                int argIndex = int.Parse(matchData.Groups[0].Value);
                _orderToIndexMapping.Add(orderIndex++, argIndex);
                // 暂时替换参数占位符为参数名信息
                formatCache.Replace(matchData.Value, argName);
            }
            // 处理format中的特殊字段
            for (int i = formatCache.Length - 1; i >= 0; i--)
            {
                // 如果包含特殊字段，则在前面插入转义符
                if (metaCharacters.Contains(formatCache[i]))
                {
                    formatCache.Insert(i, '\\');
                }
            }
            // 使用argumentPattern替换原来的参数占位符
            formatCache.Replace(argName, argumentPattern);
            this._expressionRegex = new Regex(formatCache.ToString());
        }

        // 创建只匹配数据的
        public OperatorAdapter(string argumentPattern)
        {
            
        }

        public IExpressionData ParseExpression(string expression)
        {

        }
    }
}