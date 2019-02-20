using System.Collections;
using System.Collections.Generic;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    public class SequenceGroupLocationInfoCollection : IList<SequenceGroupLocationInfo>
    {
        private readonly List<SequenceGroupLocationInfo> _innerCollection;

        public SequenceGroupLocationInfoCollection()
        {
            this._innerCollection = new List<SequenceGroupLocationInfo>(Constants.DefaultSequenceSize);
        }

        public IEnumerator<SequenceGroupLocationInfo> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(SequenceGroupLocationInfo item)
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

        public bool Contains(SequenceGroupLocationInfo item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(SequenceGroupLocationInfo[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(SequenceGroupLocationInfo item)
        {
            return _innerCollection.Contains(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(SequenceGroupLocationInfo item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, SequenceGroupLocationInfo item)
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

        public SequenceGroupLocationInfo this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }
    }
}