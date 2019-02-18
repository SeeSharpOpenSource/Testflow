using System.Collections;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    public class ParameterDataCollection : IParameterDataCollection
    {
        private readonly List<IParameterData> _innerCollection;

        public ParameterDataCollection()
        {
            this._innerCollection = new List<IParameterData>(Constants.DefaultArgumentSize);
        }

        public IEnumerator<IParameterData> GetEnumerator()
        {
            return this._innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IParameterData item)
        {
            this._innerCollection.Add(item);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(IParameterData item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(IParameterData[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IParameterData item)
        {
            throw new System.NotImplementedException();
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(IParameterData item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, IParameterData item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IParameterData this[int index]
        {
            get { return _innerCollection[index]; }
            set { throw new System.NotImplementedException(); }
        }
    }
}