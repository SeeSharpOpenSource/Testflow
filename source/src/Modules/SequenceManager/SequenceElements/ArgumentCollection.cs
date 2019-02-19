using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    internal class ArgumentCollection : IArgumentCollection
    {
        private readonly List<IArgument> _innerCollection;

        public ArgumentCollection()
        {
            this._innerCollection = new List<IArgument>(Constants.DefaultArgumentSize);
        }

        public IEnumerator<IArgument> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IArgument item)
        {
            this._innerCollection.Add(item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(IArgument item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(IArgument[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IArgument item)
        {
            return this._innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IArgument item)
        {
            return this._innerCollection.IndexOf(item);
        }

        public void Insert(int index, IArgument item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IArgument this[int index]
        {
            get { return _innerCollection[index]; }
            set { this._innerCollection[index] = value; }
        }
    }
}