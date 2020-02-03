using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.Expression
{
    public class ExpressionParser
    {
        private readonly ExpressionCalculatorInfo[] _calculatorInfos;
        private readonly ILogService _logService;
        // 包含运算符中用到的所有符号的集合
        private readonly HashSet<char> _expressionDelim;

        private readonly List<OperatorAdapter> _operatorAdapters;

        private readonly Regex _parseOverRegex;
        private readonly Regex _digitRegex;
        private readonly Regex _strRegex;
        

        internal ExpressionParser(IModuleConfigData configData, ILogService logService)
        {
            Dictionary<string, ExpressionOperatorInfo> operatorInfos =
                configData.GetProperty<Dictionary<string, ExpressionOperatorInfo>>("ExpressionOperators");
            _calculatorInfos = configData.GetProperty<ExpressionCalculatorInfo[]>("ExpressionCalculators");
            _logService = logService;
            _parseOverRegex = new Regex(Constants.SingleExpPattern, RegexOptions.Compiled);
            _digitRegex = new Regex(Constants.DigitPattern, RegexOptions.Compiled);
            _strRegex = new Regex(Constants.StringPattern, RegexOptions.Compiled);

            _expressionDelim = new HashSet<char>();
            foreach (KeyValuePair<string, ExpressionOperatorInfo> operatorInfoPair in operatorInfos)
            {
                string operatorSymbol = operatorInfoPair.Key;
                char[] symbolElem = operatorSymbol.ToCharArray();
                foreach (char elem in symbolElem)
                {
                    _expressionDelim.Add(elem);
                }
            }



            string argumentPattern = GetArgumentPattern(_expressionDelim);
            _operatorAdapters = new List<OperatorAdapter>(_operatorAdapters.Count);
            // 正则表达式中的元符号
            HashSet<char> metaCharacters = new HashSet<char>
            {
                '^', '[', '.', '$', '{', '*', '(', '\\', '+', ')','|', '?', '<', '>'
            };
            // 创建各个Operator的符号匹配器
            foreach (KeyValuePair<string, ExpressionOperatorInfo> operatorInfoPair in operatorInfos)
            {
                OperatorAdapter operatorAdapter = new OperatorAdapter(operatorInfoPair.Value, argumentPattern, metaCharacters);
                _operatorAdapters.Add(operatorAdapter);
            }
            // 按照优先级，从大到小排序
            _operatorAdapters.Sort(new OperatorAdapterComparer());
        }

        // 获取参数的pattern，使用小括号，保证可以在分组数据中找到
        private string GetArgumentPattern(HashSet<char> operatorDelims)
        {
//            StringBuilder argumentPattern = new StringBuilder();
//            argumentPattern.Append("([^");
//            foreach (char operatorDelim in operatorDelims)
//            {
//                if (_metaCharacters.Contains(operatorDelim))
//                {
//                    argumentPattern.Append('\\');
//                }
//                argumentPattern.Append(operatorDelim);
//            }
//            argumentPattern.Append("]+)");
//            return argumentPattern.ToString();
            return $"({Constants.ArgNamePattern})";
        }

        public IExpressionData ParseExpression(string expression, ISequence parent)
        {
            // 参数别名到参数值的映射
            Dictionary<string, string> argumentCache = new Dictionary<string, string>(10);
            StringBuilder expressionCache = new StringBuilder(expression);
            // 预处理，删除冗余的空格，替换参数为固定模式的字符串
            ParsingPreProcess(expressionCache, parent, argumentCache);
            // 分割表达式元素
            IExpressionData expressionData = ParseExpressionData(expressionCache);
            ParsingPostProcess(expressionData, argumentCache);
            return expressionData;
        }

        private IExpressionData ParseExpressionData(StringBuilder expressionCache)
        {
            Dictionary<string, IExpressionData> expressionDataCache = new Dictionary<string, IExpressionData>(10);
            while (!_parseOverRegex.IsMatch(expressionDataCache.ToString()))
            {
                foreach (OperatorAdapter operatorAdapter in _operatorAdapters)
                {
                    operatorAdapter.ParseExpression(expressionCache, expressionDataCache);
                }
            }
            return expressionDataCache[expressionDataCache.ToString()];
        }

        private void ParsingPreProcess(StringBuilder expressionCache, ISequence parent, Dictionary<string, string> argumentCache)
        {
            int argumentIndex = 0;
            int argEndIndex = -1;
            bool nextCharIsDelim = false;
            
            for (int i = expressionCache.Length - 1; i >= 0 ; i--)
            {
                char character = expressionCache[i];
                if (_expressionDelim.Contains(character))
                {
                    // 当前字符是运算符，且参数结束为止不为-1，则说明下一个位置是参数数据结束的位置
                    if (argEndIndex != -1)
                    {
                        CacheArgumentValue(expressionCache, ref argumentIndex, i + 1, argEndIndex, argumentCache);
                        argEndIndex = -1;
                    }
                    nextCharIsDelim = true;
                }
                // 当前节点不是运算符，下一个元素是运算符时，该字符为某个参数的结束位置
                else if (nextCharIsDelim)
                {
                    argEndIndex = i;
                    nextCharIsDelim = false;
                }
            }
            // 如果参数结束位置有效，则缓存第一个参数值
            if (argEndIndex != 1)
            {
                CacheArgumentValue(expressionCache, ref argumentIndex, 0, argEndIndex, argumentCache);
            }
        }

        private void CacheArgumentValue(StringBuilder expressionCache, ref int argIndex, int argStartIndex,
            int argEndIndex, Dictionary<string, string> argumentCache)
        {
            string argName = string.Format(Constants.ArgNameFormat, argIndex++);
            // 取出常量值，不包括引号
            int argValueLength = argEndIndex - argStartIndex + 1;
            string argumentValue = expressionCache.ToString()
                .Substring(argStartIndex, argValueLength);
            // 获取需要移除的长度，包括引号
            argumentCache.Add(argName, argumentValue);
            // 替换原来字符串位置的值为：StrX
            expressionCache.Remove(argStartIndex, argValueLength);
            expressionCache.Insert(argStartIndex, argName);
        }

        private void ParsingPostProcess(IExpressionData expressionData, Dictionary<string, string> argumentCache)
        {
            ExpressionPostProcess(expressionData, argumentCache);
        }

        private void ExpressionPostProcess(IExpressionData expressionData,
            Dictionary<string, string> argumentCache)
        {
            ExpressionElementPostProcess(expressionData.Source, argumentCache);
            foreach (IExpressionElement expressionElement in expressionData.Arguments)
            {
                ExpressionElementPostProcess(expressionElement, argumentCache);
            }
        }

        private void ExpressionElementPostProcess(IExpressionElement expressionElement, 
            Dictionary<string, string> argumentCache)
        {
            if (expressionElement.Type != ParameterType.NotAvailable)
            {
                return;
            }
            string value = argumentCache[expressionElement.Value];
            // 值是数值类型
            if (_digitRegex.IsMatch(value))
            {
                expressionElement.Value = value;
                expressionElement.Type = ParameterType.Value;
            }
            else if (_strRegex.IsMatch(value))
            {
                // 字符串替换为去除双引号后的值
                Match matchData = _strRegex.Match(value);
                expressionElement.Value = matchData.Groups[1].Value;
                expressionElement.Type = ParameterType.Value;
            }
            else
            {
                // 否则则认为表达式为变量值
                expressionElement.Value = value;
                expressionElement.Type = ParameterType.Variable;
            }
//
//            // 如果双引号或者单引号不成对，则抛出异常
//            if (inDoubleQuotation || inQuotation)
//            {
//                _logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
//                    $"Illegal expression <{expressionCache}>");
//                I18N i18N = I18N.GetInstance(Constants.I18nName);
//                throw new TestflowDataException(ModuleErrorCode.ExpressionError,
//                    i18N.GetFStr("IllegalExpression", expressionCache.ToString()));
//            }
        }
    }
}