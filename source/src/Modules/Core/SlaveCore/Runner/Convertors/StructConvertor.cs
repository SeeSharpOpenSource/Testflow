using System;
using System.Collections.Generic;
using System.Reflection;
using Testflow.CoreCommon;
using Testflow.Data;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class StructConvertor
    {
        private Dictionary<string, Func<object, object>> ConvertFuncs { get; }

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

        public bool IsValidCastTarget(Type targetType)
        {
            return ConvertFuncs.ContainsKey(targetType.Name);
        }


        public StructConvertor()
        {
            ConvertFuncs = new Dictionary<string, Func<object, object>>(1);
            ConvertFuncs.Add(typeof(string).Name, sourceValue =>
            {
                DateTime dateTime;
                bool validCast = DateTime.TryParse((string)sourceValue, out dateTime);
                if (!validCast)
                {
                    throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                        "Illegal string for datetime cast.");
                }
                return dateTime;
            });
        }

        public object GetDefaultValue(Type structType)
        {
            ConstructorInfo constructorInfo = structType.GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                null, new Type[0], null);
            if (null == constructorInfo)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.UnsupportedTypeCast, "Find struct default constructor failed.");
            }
            return constructorInfo.Invoke(null, new object[0]);
        }
    }
}