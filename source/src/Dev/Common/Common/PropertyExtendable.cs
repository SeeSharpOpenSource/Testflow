using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Testflow.Common
{
    /// <summary>
    /// 可扩展属性类的基类
    /// </summary>
    public abstract class PropertyExtendable : IPropertyExtendable
    {
        private Dictionary<string, object> _extendProperties;
        private Dictionary<string, Type> _extendPropertyTypes;

        private ConcurrentDictionary<string, object> _nameToValue;

        /// <summary>
        /// 构造长度为PropertyExtendable数量的参数集合
        /// </summary>
        /// <param name="propertyCount"></param>
        protected PropertyExtendable(int propertyCount = Constants.DefaultExtendParamCapacity)
        {
            _extendProperties = new Dictionary<string, object>(propertyCount);
            _extendPropertyTypes = new Dictionary<string, Type>(propertyCount);
            _nameToValue = new ConcurrentDictionary<string, object>(_extendProperties);
            InitializeExtendPropertyTypes();
        }

        protected abstract void InitializeExtendPropertyTypes();

        public object GetProperty(string propertyName)
        {
            return _nameToValue[propertyName];
        }

        public abstract void InitExtendProperties();

        public Dictionary<string, object> Properties { get; }

        public void SetProperty(string propertyName, object value)
        {
            this._nameToValue.TryUpdate(propertyName, value, _nameToValue[propertyName]);
        }
        public Type GetPropertyType(string propertyName)
        {
            return _extendPropertyTypes[propertyName];
        }

        public bool ContainsProperty(string propertyName)
        {
            return _extendPropertyTypes.ContainsKey(propertyName);
        }

        public IList<string> GetPropertyNames()
        {
            throw new NotImplementedException();
        }
    }
}