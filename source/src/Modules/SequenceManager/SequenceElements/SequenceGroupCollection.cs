using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceGroupCollection : ISequenceGroupCollection
    {
        private readonly IList<ISequenceGroup> _innerCollection;

        public SequenceGroupCollection()
        {
            this._innerCollection = new List<ISequenceGroup>(Constants.DefaultSequenceSize);
        }

        public IEnumerator<ISequenceGroup> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISequenceGroup item)
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

        public bool Contains(ISequenceGroup item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(ISequenceGroup[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ISequenceGroup item)
        {
            return _innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISequenceGroup item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, ISequenceGroup item)
        {
            if (_innerCollection.Contains(item))
            {
                return;
            }
            _innerCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerCollection.RemoveAt(index);
        }

        public ISequenceGroup this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new InvalidOperationException(); }
        }
    }
}