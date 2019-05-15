using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using Testflow.Usr;
using Testflow.Utility.Collections.Convertor;

namespace Testflow.Utility.Collections
{
    /// <summary>
    /// 可序列化的Map集合
    /// </summary>
    public class SerializableMap<TKey, TValue> : ISerializableMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _innerCollection;

        /// <summary>
        /// 创建序列化Map的实例
        /// </summary>
        public SerializableMap(int capacity)
        {
            this._innerCollection = new Dictionary<TKey, TValue>(capacity);
        }

        public SerializableMap(SerializationInfo info, StreamingContext context)
        {
            this._innerCollection = new Dictionary<TKey, TValue>(UtilityConstants.DefaultEntityCount);
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TKey key;
                Type keyType = typeof(TKey);
                if (keyType.IsEnum)
                {
                    key = (TKey)EnumConvertor.ReadData(keyType, enumerator.Name);
                }
                else
                {
                    key = (TKey) ValueConvertor.ReadData(keyType, enumerator.Name);
                }
                this._innerCollection.Add(key, (TValue) enumerator.Value);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this._innerCollection.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this._innerCollection.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this._innerCollection.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this._innerCollection.Remove(item.Key);
        }

        public int Count => this._innerCollection.Count;
        public bool IsReadOnly => false;
        public bool ContainsKey(TKey key)
        {
            return _innerCollection.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            this._innerCollection.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return this._innerCollection.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._innerCollection.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _innerCollection[key]; }
            set { this._innerCollection[key] = value; }
        }

        public ICollection<TKey> Keys => this._innerCollection.Keys;
        public ICollection<TValue> Values => this._innerCollection.Values;
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<TKey, TValue> keyValuePair in _innerCollection)
            {
                info.AddValue(keyValuePair.Key.ToString(), keyValuePair.Value);
            }
        }
    }
}