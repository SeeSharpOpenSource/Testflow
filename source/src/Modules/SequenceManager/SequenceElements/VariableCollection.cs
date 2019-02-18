using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
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
            if (_innerCollection.Contains(item))
            {
                return;
            }
            _innerCollection.Add(item);
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
            return this._innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IVariable item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IVariable item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IVariable this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}