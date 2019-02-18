using System.Collections;
using System.Collections.Generic;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    public class TypeDataCollection : ITypeDataCollection
    {
        private readonly List<ITypeData> _innerCollection;

        public TypeDataCollection()
        {
            this._innerCollection = new List<ITypeData>(Constants.DefaultTypeCollectionSize);
        }

        public IEnumerator<ITypeData> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ITypeData item)
        {
            if (_innerCollection.Contains(item))
            {
                return;
            }
            this._innerCollection.Add(item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(ITypeData item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(ITypeData[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ITypeData item)
        {
            return this._innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => true;
        public int IndexOf(ITypeData item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, ITypeData item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            this._innerCollection.RemoveAt(index);
        }

        public ITypeData this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}