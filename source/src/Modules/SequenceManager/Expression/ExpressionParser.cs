using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.Utils;

namespace Testflow.SequenceManager.Expression
{
    public class ExpressionParser
    {
        private const int CacheCapacity = 500;
        private readonly ExpressionCalculatorInfo[] _calculatorInfos;
        private readonly ILogService _logService;

        // 表达式解析缓存
        private readonly StringBuilder _expressionCache;
        // 参数缓存
        private Dictionary<string, string> _argumentCache;

        // 包含运算符中用到的所有符号的集合
        private readonly HashSet<char> _expressionDelim;

        private readonly List<OperatorAdapter> _operatorAdapters;
        // 解析结束的匹配模式
        private readonly Regex _parseOverRegex;
        // 数值类型参数的匹配模式
        private readonly Regex _digitRegex;
        // 字符串类型的匹配模式
        private readonly Regex _strRegex;
        // 布尔类型的匹配模式
        private readonly Regex _boolRegex;

        public ExpressionParser(IModuleConfigData configData, ILogService logService)
        {
            _expressionCache = new StringBuilder(CacheCapacity);
            _argumentCache = new Dictionary<string, string>(10);
            Dictionary<string, ExpressionOperatorInfo> operatorInfos =
                configData.GetProperty<Dictionary<string, ExpressionOperatorInfo>>("ExpressionOperators");
            _calculatorInfos = configData.GetProperty<ExpressionCalculatorInfo[]>("ExpressionCalculators");
            _logService = logService;
            _parseOverRegex = new Regex(Constants.SingleExpPattern, RegexOptions.Compiled);
            _digitRegex = new Regex(Constants.DigitPattern, RegexOptions.Compiled);
            _strRegex = new Regex(Constants.StringPattern, RegexOptions.Compiled);
            _boolRegex = new Regex(Constants.BoolPattern, RegexOptions.Compiled);

            _expressionDelim = new HashSet<char>();
            foreach (KeyValuePair<string, ExpressionOperatorInfo> operatorInfoPair in operatorInfos)
            {
                string operatorSymbol = operatorInfoPair.Value.Symbol;
                char[] symbolElem = operatorSymbol.ToCharArray();
                foreach (char elem in symbolElem)
                {
                    _expressionDelim.Add(elem);
                }
            }
            string argumentPattern = GetArgumentPattern(_expressionDelim);
            _operatorAdapters = new List<OperatorAdapter>(operatorInfos.Count);
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

        #region 表达式解析

        /// <summary>
        /// 解析表达式并校验变量
        /// </summary>
        public IExpressionData ParseExpression(string expression, ISequence parent)
        {
            // 参数别名到参数值的映射
            _argumentCache.Clear();
            _expressionCache.Clear();
            _expressionCache.Append(expression);
            // 预处理，删除冗余的空格，替换参数为固定模式的字符串
            ParsingPreProcess(_expressionCache, _argumentCache);
            // 分割表达式元素
            IExpressionData expressionData = ParseExpressionData(_expressionCache);
            ParsingPostProcess(expressionData, parent, _argumentCache);
            ResetExpressionCache();
            return expressionData;
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        public IExpressionData ParseExpression(string expression)
        {
            // 参数别名到参数值的映射
            _argumentCache.Clear();
            _expressionCache.Clear();
            try
            {
                _expressionCache.Append(expression);
                // 预处理，删除冗余的空格，替换参数为固定模式的字符串
                ParsingPreProcess(_expressionCache, _argumentCache);
                // 分割表达式元素
                IExpressionData expressionData = ParseExpressionData(_expressionCache);
                ParsingPostProcess(expressionData, null, _argumentCache);
                return expressionData;
            }
            finally
            {
                ResetExpressionCache();
            }
        }

        private IExpressionData ParseExpressionData(StringBuilder expressionCache)
        {
            Dictionary<string, IExpressionData> expressionDataCache = new Dictionary<string, IExpressionData>(10);
            string oldExpression = expressionDataCache.ToString();
            while (!_parseOverRegex.IsMatch(oldExpression))
            {
                foreach (OperatorAdapter operatorAdapter in _operatorAdapters)
                {
                    operatorAdapter.ParseExpression(expressionCache, expressionDataCache);
                }
                if (oldExpression.Equals(expressionCache.ToString()))
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.ExpressionError,
                        i18N.GetFStr("IllegalExpression", expressionCache.ToString()));
                }
                oldExpression = expressionCache.ToString();
            }
            return expressionDataCache[expressionCache.ToString()];
        }

        private void ParsingPreProcess(StringBuilder expressionCache, Dictionary<string, string> argumentCache)
        {
            int argumentIndex = 0;
            int argEndIndex = -1;
            bool nextCharIsDelim = true;
            char quoteChar = '\0';
            bool isInQuote = false;
            const char quoteChar1 = '"';
            const char quoteChar2 = '\'';
            for (int i = expressionCache.Length - 1; i >= 0; i--)
            {
                char character = expressionCache[i];

                // 如果是引号符号，则需要判断是否在引号中
                if (character == quoteChar1 || character == quoteChar2)
                {
                    // 如果在引号中，且当前符号就是最外围的引号类型，则修改状态为退出引号
                    if (isInQuote && character == quoteChar)
                    {
                        isInQuote = false;
                        quoteChar = '\0';
                    }
                    // 如果在引号外，则修改标记，进入引号范围
                    else if (!isInQuote)
                    {
                        isInQuote = true;
                        quoteChar = character;
                    }
                }

                if (_expressionDelim.Contains(character) && !isInQuote)
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
            
            string argumentValue = GetTrimmedArgValue(expressionCache, argStartIndex, argEndIndex);
            // 获取需要移除的长度，包括引号
            argumentCache.Add(argName, argumentValue);
            // 替换原来字符串位置的值为：StrX
            int argValueLength = argEndIndex - argStartIndex + 1;
            expressionCache.Remove(argStartIndex, argValueLength);
            expressionCache.Insert(argStartIndex, argName);
        }

        private string GetTrimmedArgValue(StringBuilder expressionCache, int argStartIndex, int argEndIndex)
        {
            while (_expressionCache[argStartIndex] == ' ')
            {
                argStartIndex++;
            }
            while (_expressionCache[argEndIndex] == ' ')
            {
                argEndIndex--;
            }
            int argValueLength = argEndIndex - argStartIndex + 1;
            if (argValueLength <= 0)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.ExpressionError,
                    i18N.GetFStr("IllegalExpression", expressionCache.ToString()));
            }
            string argumentValue = expressionCache.ToString(argStartIndex, argValueLength);
            return argumentValue;
        }

        private void ParsingPostProcess(IExpressionData expressionData, ISequence parent,
            Dictionary<string, string> argumentCache)
        {
            ExpressionPostProcess(expressionData, argumentCache, parent);
        }

        private void ExpressionPostProcess(IExpressionData expressionData, Dictionary<string, string> argumentCache,
            ISequence parent)
        {
            ExpressionElementPostProcess(expressionData.Source, argumentCache, parent);
            foreach (IExpressionElement expressionElement in expressionData.Arguments)
            {
                ExpressionElementPostProcess(expressionElement, argumentCache, parent);
            }
        }

        private void ExpressionElementPostProcess(IExpressionElement expressionElement,
            Dictionary<string, string> argumentCache, ISequence parent)
        {
            if (expressionElement.Type == ParameterType.NotAvailable)
            {
                return;
            }
            if (expressionElement.Type == ParameterType.Expression)
            {
                ExpressionPostProcess(expressionElement.Expression, argumentCache, parent);
                return;
            }
            string value = argumentCache[expressionElement.Value];
            // 值是数值类型或布尔类型
            if (_digitRegex.IsMatch(value) || _boolRegex.IsMatch(value))
            {
                expressionElement.Value = value;
                expressionElement.Type = ParameterType.Value;
            }
            else if (_strRegex.IsMatch(value))
            {
                // 字符串替换为去除双引号后的值
                Match matchData = _strRegex.Match(value);
                expressionElement.Value = matchData.Groups[2].Value;
                expressionElement.Type = ParameterType.Value;
            }
            else
            {
                if (null != parent && !SequenceUtils.IsVariableExist(value, parent))
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.ExpressionError, i18N.GetFStr("ExpVariableNotExist", value));
                }
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

        #endregion

        public bool RenameVariable(string expression, string varOldName, string varNewName)
        {
            if (!expression.Contains(varOldName))
            {
                return false;
            }
            int index = expression.Length;
            _expressionCache.Clear();
            _expressionCache.Append(expression);
            const char empty = '\0';
            int oldNameLength = varOldName.Length;
            bool varExist = false;
            while ((index = expression.LastIndexOf(varOldName, 0, index, StringComparison.Ordinal)) >= 0)
            {
                // 找出对应变量名左侧和右侧第一个非空格的字符
                char leftElement;
                int leftIndex = index - 1;
                do
                {
                    leftElement = leftIndex >= 0 ? _expressionCache[leftIndex] : empty;
                    leftIndex--;
                } while (' ' == leftElement);
                char rightElement;
                int rightIndex = index + oldNameLength;
                do
                {
                    rightElement = rightIndex < _expressionCache.Length ? _expressionCache[rightIndex] : empty;
                    rightIndex++;
                } while (' ' == rightElement);
                // 如果左侧元素或右侧元素是计算分隔符或者空字符，则认为此处为变量
                // TODO 这里可能会出现字符串中有变量名且前后字符都是运算符的问题，该场景少见，后续优化
                if ((_expressionDelim.Contains(leftElement) || leftElement == empty) &&
                    (_expressionDelim.Contains(rightElement) || rightElement == empty))
                {
                    _expressionCache.Replace(varOldName, varNewName, index, oldNameLength);
                    varExist = true;
                }
            }
            return varExist;
        }


        private void ResetExpressionCache()
        {
            _expressionCache.Clear();
            if (_expressionCache.Capacity > CacheCapacity)
            {
                _expressionCache.Capacity = CacheCapacity;
            }
            _argumentCache.Clear();
        }
    }
}