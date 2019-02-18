using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    public class SequenceStepCollection : ISequenceStepCollection
    {
        private readonly List<ISequenceStep> _innerCollection;

        public SequenceStepCollection()
        {
            this._innerCollection = new List<ISequenceStep>(Constants.DefaultSequenceSize);
        }

        public IEnumerator<ISequenceStep> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISequenceStep item)
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

        public bool Contains(ISequenceStep item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(ISequenceStep[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ISequenceStep item)
        {
            return _innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISequenceStep item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, ISequenceStep item)
        {
            if (_innerCollection.Contains(item))
            {
                return;
            }
            _innerCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this._innerCollection.RemoveAt(index);
        }

        public ISequenceStep this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}