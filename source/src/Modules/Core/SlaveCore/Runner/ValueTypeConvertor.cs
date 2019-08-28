using System;
using System.Collections.Generic;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Convertors;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner
{
    internal class ValueTypeConvertor
    {
        private readonly SlaveContext _context;
        private readonly Dictionary<string, ValueConvertorBase> _convertors;
        private readonly ValueConvertorBase _strConvertor;

        public ValueTypeConvertor(SlaveContext context)
        {
            _context = context;
            _convertors = new Dictionary<string, ValueConvertorBase>(20)
            {
                {typeof (decimal).Name, new DecimalConvertor()},
                {typeof (double).Name, new DoubleConvertor()},
                {typeof (float).Name, new FloatConvertor()},
                {typeof (long).Name, new LongConvertor()},
                {typeof (ulong).Name, new ULongConvertor()},
                {typeof (int).Name, new IntConvertor()},
                {typeof (uint).Name, new UIntConvertor()},
                {typeof (short).Name, new ShortConvertor()},
                {typeof (ushort).Name, new UShortConvertor()},
                {typeof (char).Name, new CharConvertor()},
                {typeof (byte).Name, new ByteConvertor()},
                {typeof (bool).Name, new BoolConvertor()},
                {typeof (string).Name, new StringConvertor()}
            };
            _strConvertor = _convertors[typeof (string).Name];
        }

        public object CastValue(ITypeData targetType, object sourceValue)
        {
            if (null == sourceValue)
            {
                _context.LogSession.Print(LogLevel.Warn, _context.SessionId, "Cannot cast null value.");
                return null;
            }
            Type sourceType = sourceValue.GetType();
            if (!IsValidCast(sourceType, targetType))
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId, 
                    $"Unsupported type cast from type <{sourceType.Name}> to type <{targetType.Name}>.");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast, 
                    _context.I18N.GetFStr("InvalidValueTypeCast", sourceType.Name, targetType.Name));
            }
            return _convertors[sourceType.Name].CastValue(targetType, sourceValue);
        }

        public object CastValue(Type targetType, object sourceValue)
        {
            if (null == sourceValue)
            {
                _context.LogSession.Print(LogLevel.Warn, _context.SessionId, "Cannot cast null value.");
                return null;
            }
            Type sourceType = sourceValue.GetType();
            if (!IsValidCast(sourceType, targetType))
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId,
                    $"Unsupported type cast from type <{sourceType.Name}> to type <{targetType.Name}>.");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("InvalidValueTypeCast", sourceType.Name, targetType.Name));
            }
            return _convertors[sourceType.Name].CastValue(targetType, sourceValue);
        }

        /// <summary>
        /// 字符串转换为值类型
        /// </summary>
        public object CastConstantValue(Type targetType, string sourceValue)
        {
            if (targetType == typeof(string))
            {
                return sourceValue;
            }
            else if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, sourceValue);
            }
            else if (targetType.IsValueType)
            {
                return _strConvertor.CastValue(targetType, sourceValue);
            }
            throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                _context.I18N.GetFStr("InvalidTypeCast", targetType.Name));
        }

        public object GetDefaultValue(ITypeData type)
        {
            return _convertors.ContainsKey(type.Name) ? _convertors[type.Name].GetDefaultValue() : null;
        }
        
        private bool IsValidCast(Type sourceType, ITypeData targetType)
        {
            return _convertors.ContainsKey(sourceType.Name) &&
                   _convertors[sourceType.Name].IsValidCastTarget(targetType);
        }

        private bool IsValidCast(Type sourceType, Type targetType)
        {
            return _convertors.ContainsKey(sourceType.Name) &&
                   _convertors[sourceType.Name].IsValidCastTarget(targetType);
        }

        public bool NeedCastValue(object sourceValue, ITypeData targetType)
        {
            return (null != sourceValue &&
                    ModuleUtils.GetTypeFullName(targetType) == ModuleUtils.GetTypeFullName(sourceValue.GetType())) || 
                   (null == sourceValue && _convertors.ContainsKey(targetType.Name));
        }
    }
}