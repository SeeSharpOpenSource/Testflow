using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Expression;
using Testflow.SequenceManager.Common;
using static Testflow.SequenceManager.Common.ModuleUtils;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [GenericCollection(typeof(Expression.ExpressionData))]
    public class ExpressionCollection : IExpressionCollection
    {
        private readonly List<IExpressionData> _innerCollection;

        public ExpressionCollection()
        {
            this._innerCollection = new List<IExpressionData>(Constants.DefaultTypeCollectionSize);
        }

        public IEnumerator<IExpressionData> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IExpressionData item)
        {
            SetElementName(item, this);
            AddAndRefreshIndex(_innerCollection, item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(IExpressionData item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(IExpressionData[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IExpressionData item)
        {
            return RemoveAndRefreshIndex(_innerCollection, item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IExpressionData item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IExpressionData item)
        {
            SetElementName(item, this);
            InsertAndRefreshIndex(_innerCollection, item, index);
        }

        public void RemoveAt(int index)
        {
            RemoveAtAndRefreshIndex(_innerCollection, index);
        }

        public IExpressionData this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }
    }
}