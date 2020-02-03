using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

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
            Dictionary<string, IExpressionElement> argumentCache = new Dictionary<string, IExpressionElement>(10);
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

        private void ParsingPreProcess(StringBuilder expressionCache, ISequence parent, Dictionary<string, IExpressionElement> argumentCache)
        {
            const char quotation = '\'';
            const char doubleQuotation = '"';
            const char whitespace = ' ';
            bool inQuotation = false;
            bool inDoubleQuotation = false;
            int doubleQuotationEndIndex = -1;
            int quotationEndIndex = -1;
            int argumentIndex = 0;
            int argEndIndex = -1;
            bool nextCharIsDelim = false;
            
            for (int i = expressionCache.Length - 1; i >= 0 ; i--)
            {
                char character = expressionCache[i];
                // 如果在引号中，且当前character不是结束符，则跳过当前字符执行
                if ((!character.Equals(doubleQuotation) && inDoubleQuotation) ||
                    (!character.Equals(quotation) && inQuotation))
                {
                    continue;
                }
                if (whitespace.Equals(character))
                {
                    // 删除所有没有包含在单引号和双引号中的空格字符
                    expressionCache.Remove(i, 1);
                }
                else if (character.Equals(doubleQuotation) && !inQuotation)
                {
                    inDoubleQuotation = !inDoubleQuotation;
                    if (inDoubleQuotation)
                    {
                        doubleQuotationEndIndex = i;
                    }
                    else
                    {
                        CacheConstString(expressionCache, ref argumentIndex, i, doubleQuotationEndIndex, argumentCache);
                        // 复位结束为止索引
                        doubleQuotationEndIndex = -1;
                    }
                }
                else if (character.Equals(quotation) && !inDoubleQuotation)
                {
                    inQuotation = !inQuotation;
                    if (inQuotation)
                    {
                        quotationEndIndex = i;
                    }
                    else
                    {
                        CacheConstString(expressionCache, ref argumentIndex, i, quotationEndIndex, argumentCache);
                        // 复位结束为止索引
                        quotationEndIndex = -1;
                    }
                }
                else if (_expressionDelim.Contains(character))
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
            // 如果双引号或者单引号不成对，则抛出异常
            if (inDoubleQuotation || inQuotation)
            {
                _logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                    $"Quotation Error in expression <{expressionCache}>");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.ExpressionError,
                    i18N.GetFStr("IllegalExpression", expressionCache.ToString()));
            }
        }

        private void CacheConstString(StringBuilder expressionCache, ref int argIndex, int startQuotationIndex,
            int endQuotationIndex, Dictionary<string, IExpressionElement> argumentCache)
        {
            string argName = string.Format(Constants.ArgNameFormat, argIndex++);
            int strStartIndex = startQuotationIndex + 1;
            // 取出常量值，不包括引号
            string constStrValue = expressionCache.ToString()
                .Substring(strStartIndex, endQuotationIndex - strStartIndex);
            // 获取需要移除的长度，包括引号
            int replaceLength = endQuotationIndex - startQuotationIndex + 1;
//                        string replaceStrValue = expression.Substring(i, replaceLength);
            argumentCache.Add(argName, new ExpressionElement(ParameterType.Value, constStrValue));
            // 替换原来字符串位置的值为：StrX
            expressionCache.Remove(startQuotationIndex, replaceLength);
            expressionCache.Insert(startQuotationIndex, argName);
        }

        private void CacheArgumentValue(StringBuilder expressionCache, ref int argIndex, int argStartIndex, int argEndIndex, Dictionary<string, IExpressionElement> argumentCache)
        {
            string argName = string.Format(Constants.ArgNameFormat, argIndex++);
            // 取出常量值，不包括引号
            int argValueLength = argEndIndex - argStartIndex + 1;
            string argumentValue = expressionCache.ToString()
                .Substring(argStartIndex, argValueLength);
            // 获取需要移除的长度，包括引号
            argumentCache.Add(argName, new ExpressionElement(ParameterType.Value, argumentValue));
            // 替换原来字符串位置的值为：StrX
            expressionCache.Remove(argStartIndex, argValueLength);
            expressionCache.Insert(argStartIndex, argName);
        }

        private void ParsingPostProcess(IExpressionData expressionData, Dictionary<string, IExpressionElement> argumentCache)
        {

        }

        private void ExpressionPostProcess(IExpressionData expressionData,
            Dictionary<string, IExpressionElement> argumentCache)
        {
            ExpressionElementPostProcess(expressionData.Source, argumentCache);
            foreach (IExpressionElement expressionElement in expressionData.Arguments)
            {
                ExpressionElementPostProcess(expressionElement, argumentCache);
            } 
        }

        private void ExpressionElementPostProcess(IExpressionElement expressionElement, 
            Dictionary<string, IExpressionElement> argumentCache)
        {
            if (expressionElement.Type != ParameterType.NotAvailable)
            {
                return;
            }
            IExpressionElement value = argumentCache[expressionElement.Value];
        }
    }
}