using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class VariableInitValueCollection : IList<IVariableInitValue>
    {
        private readonly List<IVariableInitValue> _innerCollection;

        public VariableInitValueCollection()
        {
            this._innerCollection = new List<IVariableInitValue>(Constants.UnverifiedTypeIndex);
        }

        public IEnumerator<IVariableInitValue> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IVariableInitValue item)
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

        public bool Contains(IVariableInitValue item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(IVariableInitValue[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IVariableInitValue item)
        {
            return _innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IVariableInitValue item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IVariableInitValue item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IVariableInitValue this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}