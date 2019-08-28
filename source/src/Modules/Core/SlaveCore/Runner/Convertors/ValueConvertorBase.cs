using System;
using System.Collections.Generic;
using Testflow.Data;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal abstract class ValueConvertorBase
    {
        protected Dictionary<string, Func<object, object>> ConvertFuncs { get; }

        protected ValueConvertorBase()
        {
            ConvertFuncs = new Dictionary<string, Func<object, object>>(20);
            InitializeConvertFuncs();
        }

        protected abstract void InitializeConvertFuncs();

        public abstract object GetDefaultValue();

        public object CastValue(ITypeData targetType, object sourceValue)
        {
            return ConvertFuncs[targetType.Name].Invoke(sourceValue);
        }

        public object CastValue(Type targetType, object sourceValue)
        {
            return ConvertFuncs[targetType.Name].Invoke(sourceValue);
        }

        public bool IsValidCastTarget(ITypeData targetType)
        {
            return ConvertFuncs.ContainsKey(targetType.Name);
        }
    }
}