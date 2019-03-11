using System;
using System.Collections;
using System.Collections.Generic;

namespace Testflow.EngineCore.Common
{
    [Serializable]
    public class SerializableList<TDataType> : IList<TDataType>
    {
        private readonly List<TDataType> _innerList;

        public SerializableList()
        {
            this._innerList = new List<TDataType>(Constants.DefaultRuntimeSize);
        }

        public IEnumerator<TDataType> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TDataType item)
        {
            _innerList.Add(item);
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Contains(TDataType item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(TDataType[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TDataType item)
        {
            throw new NotImplementedException();
        }

        public int Count => _innerList.Count;
        public bool IsReadOnly => false;
        public int IndexOf(TDataType item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, TDataType item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public TDataType this[int index]
        {
            get { return _innerList[index]; }
            set { throw new NotImplementedException(); }
        }
    }
}