using System;
using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [GenericCollection(typeof(ParameterDataCollection))]
    public class ParameterDataCollections : IList<IParameterDataCollection>
    {
        private readonly List<IParameterDataCollection> _innerCollection;

        public ParameterDataCollections()
        {
            this._innerCollection = new List<IParameterDataCollection>(Constants.DefaultArgumentSize);
        }

        public IEnumerator<IParameterDataCollection> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IParameterDataCollection item)
        {
            Common.Utility.AddAndRefreshIndex(this._innerCollection, item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(IParameterDataCollection item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(IParameterDataCollection[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IParameterDataCollection item)
        {
            throw new System.NotImplementedException();
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IParameterDataCollection item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IParameterDataCollection item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IParameterDataCollection this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}