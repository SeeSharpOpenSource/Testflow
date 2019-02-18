using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceParameterCollection : IList<ISequenceParameter>
    {
        private readonly List<ISequenceParameter> _innerCollection;

        public SequenceParameterCollection()
        {
            this._innerCollection = new List<ISequenceParameter>(Constants.DefaultArgumentSize);
        }

        public IEnumerator<ISequenceParameter> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISequenceParameter item)
        {
            Common.Utility.AddAndRefreshIndex(_innerCollection, item);
        }

        public void Clear()
        {
            _innerCollection.Clear();
        }

        public bool Contains(ISequenceParameter item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(ISequenceParameter[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ISequenceParameter item)
        {
            return Common.Utility.RemoveAndRefreshIndex(_innerCollection, item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISequenceParameter item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, ISequenceParameter item)
        {
            Common.Utility.InsertAndRefreshIndex(_innerCollection, item, index);
        }

        public void RemoveAt(int index)
        {
            Common.Utility.RemoveAtAndRefreshIndex(_innerCollection, index);
        }

        public ISequenceParameter this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }
    }
}