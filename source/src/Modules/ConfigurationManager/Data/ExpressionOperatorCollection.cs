using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Testflow.Data.Expression;

namespace Testflow.ConfigurationManager.Data
{
    /// <summary>
    /// 表达式描述信息集合
    /// </summary>
    [Serializable]
    public class ExpressionOperatorCollection : IExpressionOperatorCollection
    {
        private readonly List<IExpressionOperatorInfo> _innerCollection;

        internal ExpressionOperatorCollection(IEnumerable<ExpressionOperatorInfo> operators)
        {
            this._innerCollection = new List<IExpressionOperatorInfo>(operators.Count());
            foreach (ExpressionOperatorInfo operatorInfo in operators)
            {
                _innerCollection.Add(operatorInfo);
            }
        }

        public IEnumerator<IExpressionOperatorInfo> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IExpressionOperatorInfo item)
        {
            _innerCollection.Add(item);
        }

        public void Clear()
        {
            _innerCollection.Clear();
        }

        public bool Contains(IExpressionOperatorInfo item)
        {
            return _innerCollection.Contains(item) ||
                   (null != item && _innerCollection.Any(element => element.Symbol.Equals(item.Symbol)));
        }

        public void CopyTo(IExpressionOperatorInfo[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IExpressionOperatorInfo item)
        {
            return _innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IExpressionOperatorInfo item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IExpressionOperatorInfo item)
        {
            _innerCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerCollection.RemoveAt(index);
        }

        public IExpressionOperatorInfo this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new NotImplementedException(); }
        }

        public IExpressionOperatorInfo GetOperatorInfo(string operatorToken)
        {
            return _innerCollection.FirstOrDefault(item => item.Symbol.Equals(operatorToken));
        }

        public IExpressionOperatorInfo GetOperatorInfoByName(string operatorName)
        {
            return _innerCollection.FirstOrDefault(item => item.Name.Equals(operatorName));
        }
    }
}