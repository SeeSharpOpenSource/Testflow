using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [GenericCollection(typeof(SequenceStep))]
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
            ModuleUtils.SetElementName(item, this);
            ModuleUtils.AddAndRefreshIndex(_innerCollection, item);
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
            return ModuleUtils.RemoveAndRefreshIndex(_innerCollection, item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISequenceStep item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, ISequenceStep item)
        {
            ModuleUtils.SetElementName(item, this);
            ModuleUtils.InsertAndRefreshIndex(_innerCollection, item, index);
        }

        public void RemoveAt(int index)
        {
            ModuleUtils.RemoveAtAndRefreshIndex(_innerCollection, index);
        }

        public ISequenceStep this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}