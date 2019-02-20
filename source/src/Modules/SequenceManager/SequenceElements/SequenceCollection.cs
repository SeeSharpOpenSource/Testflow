using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceCollection : ISequenceCollection
    {
        private readonly IList<ISequence> _innerCollection;

        public SequenceCollection()
        {
            this._innerCollection = new List<ISequence>(Constants.DefaultSequenceSize);
        }

        public IEnumerator<ISequence> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISequence item)
        {
            Common.Utility.AddAndRefreshIndex(_innerCollection, item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(ISequence item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(ISequence[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ISequence item)
        {
            return Common.Utility.RemoveAndRefreshIndex(_innerCollection, item);
        }

        public int Count => this._innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISequence item)
        {
            return this._innerCollection.IndexOf(item);
        }

        public void Insert(int index, ISequence item)
        {
            Common.Utility.InsertAndRefreshIndex(_innerCollection, item, index);
        }

        public void RemoveAt(int index)
        {
            Common.Utility.RemoveAtAndRefreshIndex(_innerCollection, index);
        }

        public ISequence this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }
    }
}