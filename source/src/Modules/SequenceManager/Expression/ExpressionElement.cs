using System;
using System.Runtime.Serialization;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.Expression
{
    /// <summary>
    /// 表达式元素
    /// </summary>
    [Serializable]
    public class ExpressionElement : IExpressionElement
    {
        private ParameterType _type;
        /// <summary>
        /// 表达式元素的类型
        /// </summary>
        public ParameterType Type { get { return _type; }
            set
            {
                this._type = value;
                switch (_type)
                {
                    case ParameterType.NotAvailable:
                        this._value = string.Empty;
                        this._expression = null;
                        break;
                    case ParameterType.Value:
                    case ParameterType.Variable:
                        this._expression = null;
                        break;
                    case ParameterType.Expression:
                        this._value = string.Empty;
                        break;
                    default:
                        break;
                }
            }
        }

        private string _value;
        /// <summary>
        /// 表达式元素的值，仅在Type为Value或Variable时生效
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (Type == ParameterType.Value || Type == ParameterType.Variable)
                {
                    this._value = value;
                }
            }
        }

        private IExpressionData _expression;
        /// <summary>
        /// 表达式元素的值，仅在Type为Expression时生效
        /// </summary>
        public IExpressionData Expression
        {
            get { return _expression; }
            set
            {
                if (Type == ParameterType.Expression)
                {
                    this._expression = value;
                }
            }
        }

        private ISequenceFlowContainer _parent;

        public ISequenceDataContainer Clone()
        {
            ExpressionElement element = new ExpressionElement()
            {
                Type = this.Type,
                Value = this.Value,
                Expression = this.Expression
            };
            return element;
        }

        public ExpressionElement()
        {
            this._type = ParameterType.NotAvailable;
            this._value = string.Empty;
            this._expression = null;
            this._parent = null;
        }

        public ExpressionElement(ParameterType type, string value)
        {
            this._type = type;
            this._value = value;
            this._expression = null;
            this._parent = null;
        }

        public ExpressionElement(IExpressionData value)
        {
            this._type = ParameterType.Expression;
            this._value = string.Empty;
            this._expression = value;
            this._parent = null;
        }

        public ExpressionElement(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type, typeof(ParameterType));
            switch (Type)
            {
                case ParameterType.NotAvailable:
                    break;
                case ParameterType.Value:
                case ParameterType.Variable:
                    info.AddValue("Value", Value);
                    break;
                case ParameterType.Expression:
                    info.AddValue("Expression", Expression, typeof(ExpressionData));
                    break;
                default:
                    break;
            }
        }

        public void Initialize(ISequenceFlowContainer parent)
        {
            this._parent = parent;
            if (null != parent && Type == ParameterType.Expression)
            {
                Expression?.Initialize(parent);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Type = (ParameterType) info.GetValue("Type", typeof (ParameterType));
            switch (Type)
            {
                case ParameterType.NotAvailable:
                    break;
                case ParameterType.Value:
                case ParameterType.Variable:
                    this.Value = info.GetString("Value");
                    break;
                case ParameterType.Expression:
                    this.Expression = (ExpressionData) info.GetValue("Expression", typeof (ExpressionData));
                    break;
                default:
                    break;
            }
        }
    }
}