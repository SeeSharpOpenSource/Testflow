using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class AssemblyInfoCollection : IAssemblyInfoCollection
    {
        private readonly List<IAssemblyInfo> _innerCollection;

        public AssemblyInfoCollection()
        {
            this._innerCollection = new List<IAssemblyInfo>(Constants.DefaultTypeCollectionSize);
        }

        public IEnumerator<IAssemblyInfo> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IAssemblyInfo item)
        {
            if (_innerCollection.Contains(item))
            {
                return;
            }
            _innerCollection.Add(item);
        }

        public void Clear()
        {
            _innerCollection.Clear();
        }

        public bool Contains(IAssemblyInfo item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(IAssemblyInfo[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IAssemblyInfo item)
        {
            return _innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IAssemblyInfo item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IAssemblyInfo item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IAssemblyInfo this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}