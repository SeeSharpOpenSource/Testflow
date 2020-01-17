using System;
using System.Runtime.Serialization;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Usr;

namespace Testflow.SequenceManager.Expression
{
    /// <summary>
    /// 表达式数据
    /// </summary>
    [Serializable]
    public class ExpressionData : IExpressionData
    {
        /// <summary>
        /// 表达式的名称
        /// </summary>
        public string Name { get; set; }

        public ISequence Parent { get; set; }

        private IExpressionElement _source;
        /// <summary>
        /// 表达式中的源
        /// </summary>
        public IExpressionElement Source
        {
            get { return _source; }
            set
            {
                this._source = value;
                if (null != Parent && null != _source)
                {
                    this._source.Initialize(Parent);
                }
            }
        }

        private IExpressionElement _target;
        /// <summary>
        /// 表达式中的目标
        /// </summary>
        public IExpressionElement Target
        {
            get { return _target; }
            set
            {
                this._target = value;
                if (null != Parent && null == _source)
                {
                    _target.Initialize(Parent);
                }
            }
        }

        /// <summary>
        /// 表达式的名称
        /// </summary>
        public string Operation { get; set; }

        public ExpressionData()
        {
            this.Name = string.Empty;
            this.Source = new ExpressionElement();
            this.Target = new ExpressionElement();
            this.Operation = CommonConst.NAOperator;
            this.Parent = null;
        }

        public ExpressionData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name, typeof(string));
            info.AddValue("Source", Source, typeof(ExpressionElement));
            info.AddValue("Target", Target, typeof(ExpressionElement));
            info.AddValue("Operation", Operation, typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.Name = info.GetString("Name");
            this.Source = (IExpressionElement) info.GetValue("Source", typeof(ExpressionElement));
            this.Target = (IExpressionElement) info.GetValue("Target", typeof(ExpressionElement));
            this.Operation = info.GetString("Operation");
        }

        public ISequenceDataContainer Clone()
        {
            ExpressionData data = new ExpressionData()
            {
                Name = string.Empty,
                Operation = this.Operation,
                Source = (IExpressionElement) Source.Clone(),
                Target = (IExpressionElement) Target.Clone(),
            };
            return data;
        }

        public void Initialize(ISequenceFlowContainer parent)
        {
            this.Parent = (ISequence) parent;
            if (null != parent)
            {
                this.Source?.Initialize(Parent);
                this.Target?.Initialize(Parent);
            }
        }
    }
}