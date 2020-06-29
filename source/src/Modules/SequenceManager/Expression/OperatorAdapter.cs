using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Expression
{
    internal class OperatorAdapter
    {
        // 表达式占位符的模式
        private static Regex _expPlaceHodlerRegex;

        static OperatorAdapter()
        {
            _expPlaceHodlerRegex = new Regex(Constants.ExpPlaceHodlerPattern);
        }

        /// <summary>
        /// 运算的优先级
        /// </summary>
        public int Priority => _operatorInfo.Priority;

        // 保存表达式中占位符的位置到参数索引的映射
        private readonly Dictionary<int, int> _orderToIndexMapping;

        // 表达式的匹配模式
        private readonly Regex _expressionRegex;

        // 参数的个数，包括source
        private readonly int _paramCount;

        private readonly ExpressionOperatorInfo _operatorInfo;

        public OperatorAdapter(ExpressionOperatorInfo operatorInfo, string argumentPattern, HashSet<char> metaCharacters)
        {
            this._operatorInfo = operatorInfo;
            const string argName = "ArgOfExp";
            StringBuilder formatCache = new StringBuilder(operatorInfo.FormatString, 200);
            Regex argsRegex = new Regex("\\{([0-9]+)\\}");
            MatchCollection matches = argsRegex.Matches(operatorInfo.FormatString);
            _orderToIndexMapping = new Dictionary<int, int>(matches.Count);
            int orderIndex = 0;
            // 缓存表达式中参数顺序到参数编号的映射，0为Source
            foreach (Match matchData in matches)
            {
                int argIndex = int.Parse(matchData.Groups[1].Value);
                _orderToIndexMapping.Add(orderIndex++, argIndex);
                // 暂时替换参数占位符为参数名信息
                formatCache.Replace(matchData.Value, argName);
            }
            _paramCount = orderIndex;
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
            // 注：为了保证最后的表达式的Source为最小化的，需要表达式从右向左匹配
            this._expressionRegex = new Regex(formatCache.ToString(), RegexOptions.RightToLeft);
        }

        // 创建只匹配数据的
        public OperatorAdapter(string argumentPattern)
        {
            
        }

        // 处理表达式，如果有匹配的项，处理后添加到表达式缓存中，同步处理后的表达式字符串到expression的缓存中
        public bool ParseExpression(StringBuilder expression, Dictionary<string, IExpressionData> expressionCache)
        {
            string expressionStr = expression.ToString();
            MatchCollection matches = _expressionRegex.Matches(expressionStr);
            if (matches.Count <= 0)
            {
                return false;
            }
            foreach (Match match in matches)
            {
                // group中第一个元素是匹配到的字符串本身
                if (match.Groups.Count != _paramCount + 1)
                {
                    LogAndThrowExpressionError($"Illegal expression string {expressionStr}", 
                        ModuleErrorCode.ExpressionError, "IllegalExpression", expressionStr);
                }
                string expPlaceHolder = ProcessSingleMatch(match, expressionCache);
                expression.Replace(match.Value, expPlaceHolder);
            }
            return true;
        }

        // 处理表达式，并返回该表达式在表达式缓存中的Key
        private string ProcessSingleMatch(Match matchData, Dictionary<string, IExpressionData> expressionCache)
        {
            // paramCount中还包含source的计数，所以需要删掉
            ExpressionData expression = new ExpressionData(_paramCount - 1)
            {
                Name = _operatorInfo.Name
            };
            
            foreach (int orderIndex in _orderToIndexMapping.Keys)
            {
                // Group中索引为0的元素是全局匹配到的值，groups中的索引等于真实索引号是加一
                int groupIndex = orderIndex + 1;
                IExpressionElement expressionElement = GetExpressionElement(matchData.Groups[groupIndex].Value,
                    expressionCache);
                int argIndex = _orderToIndexMapping[orderIndex];
                if (0 == argIndex)
                {
                    expression.Source = expressionElement;
                }
                else
                {
                    expression.Arguments.Add(expressionElement);
                }
            }
            string expPlaceHolder = GetExpPlaceHolder(expressionCache);
            expressionCache.Add(expPlaceHolder, expression);
            return expPlaceHolder;
        }

        private IExpressionElement GetExpressionElement(string value, Dictionary<string, IExpressionData> expressionCache)
        {
            IExpressionElement expElement;
            // 如果表达式的值已经被缓存，则配置该表达式元素的值为表达式
            if (expressionCache.ContainsKey(value))
            {
                expElement = new ExpressionElement(expressionCache[value]);
                expressionCache.Remove(value);
            }
            // 如果参数值不是表达式，则将该参数占位符填充到表达式中
            else
            {
                expElement = new ExpressionElement(ParameterType.NotAvailable, value);
            }
            return expElement;
        }

        private string GetExpPlaceHolder(Dictionary<string, IExpressionData> expressionCache)
        {
            int index = 0;
            string placeHolder = null;
            do
            {
                placeHolder = string.Format(Constants.ExpPlaceHodlerFormat, index++);
            } while (expressionCache.ContainsKey(placeHolder));
            return placeHolder;
        }

        private void LogAndThrowExpressionError(string logMessage, int errorCode, string errorLabel, params string[] extraParams)
        {
            TestflowRunner runnerInstance = TestflowRunner.GetInstance();
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            runnerInstance.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, logMessage);
            throw new TestflowDataException(errorCode, i18N.GetFStr(errorLabel, extraParams));
        }
    }
}