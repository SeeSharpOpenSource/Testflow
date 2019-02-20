﻿using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    internal class SequenceStepParameterCollection : IList<ISequenceStepParameter>
    {
        private readonly List<ISequenceStepParameter> _innerCollection;

        public SequenceStepParameterCollection()
        {
            this._innerCollection = new List<ISequenceStepParameter>(Constants.DefaultArgumentSize);
        }

        public IEnumerator<ISequenceStepParameter> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISequenceStepParameter item)
        {
            Common.Utility.AddAndRefreshIndex(_innerCollection, item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(ISequenceStepParameter item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(ISequenceStepParameter[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ISequenceStepParameter item)
        {
            return Common.Utility.RemoveAndRefreshIndex(_innerCollection, item);
        }

        public int Count => this._innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISequenceStepParameter item)
        {
            return this._innerCollection.IndexOf(item);
        }

        public void Insert(int index, ISequenceStepParameter item)
        {
            Common.Utility.InsertAndRefreshIndex(_innerCollection, item, index);
        }

        public void RemoveAt(int index)
        {
            Common.Utility.RemoveAtAndRefreshIndex(_innerCollection, index);
        }

        public ISequenceStepParameter this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}