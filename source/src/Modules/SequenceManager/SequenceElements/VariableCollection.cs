using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;
using static Testflow.SequenceManager.Common.ModuleUtils;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [GenericCollection(typeof(Variable))]
    public class VariableCollection : IVariableCollection
    {
        private readonly List<IVariable> _innerCollection;

        public VariableCollection()
        {
            this._innerCollection = new List<IVariable>(Constants.DefaultTypeCollectionSize);
        }

        public IEnumerator<IVariable> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IVariable item)
        {
            SetElementName(item, this);
            AddAndRefreshIndex(_innerCollection, item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(IVariable item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(IVariable[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IVariable item)
        {
            return RemoveAndRefreshIndex(_innerCollection, item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IVariable item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IVariable item)
        {
            SetElementName(item, this);
            InsertAndRefreshIndex(_innerCollection, item, index);
        }

        public void RemoveAt(int index)
        {
            RemoveAtAndRefreshIndex(_innerCollection, index);
        }

        public IVariable this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }
    }
}