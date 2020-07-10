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
    internal class TypeConvertor
    {
        private readonly SlaveContext _context;
        private readonly Dictionary<string, ValueConvertorBase> _convertors;
        // 非值类型转换器，仅用于静态入参类型的转换
        private readonly NonValueTypeConvertor _nonValueConvertor;
        private readonly ValueConvertorBase _strConvertor;

        public TypeConvertor(SlaveContext context)
        {
            _context = context;
            string numericFormat = context.GetPropertyString("NumericFormat");
            _convertors = new Dictionary<string, ValueConvertorBase>(20)
            {
                {typeof (decimal).Name, new DecimalConvertor(numericFormat)},
                {typeof (double).Name, new DoubleConvertor(numericFormat)},
                {typeof (float).Name, new FloatConvertor(numericFormat)},
                {typeof (long).Name, new LongConvertor()},
                {typeof (ulong).Name, new ULongConvertor()},
                {typeof (int).Name, new IntConvertor()},
                {typeof (uint).Name, new UIntConvertor()},
                {typeof (short).Name, new ShortConvertor()},
                {typeof (ushort).Name, new UShortConvertor()},
                {typeof (char).Name, new CharConvertor()},
                {typeof (byte).Name, new ByteConvertor()},
                {typeof (bool).Name, new BoolConvertor()},
                {typeof (string).Name, new StringConvertor()},
                {typeof (DateTime).Name, new DateTimeConvertor()}
            };
            _strConvertor = _convertors[typeof (string).Name];
            _nonValueConvertor = new NonValueTypeConvertor(_context);
        }

        public object CastValue(ITypeData targetType, object sourceValue)
        {
            if (null == sourceValue)
            {
                _context.LogSession.Print(LogLevel.Warn, _context.SessionId, "Cannot cast null value.");
                return null;
            }
            Type sourceType = sourceValue.GetType();
            if (IsNeedNoConvert(sourceType, targetType))
            {
                return sourceValue;
            }
            if (!IsValidValueCast(sourceType, targetType))
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId, 
                    $"Unsupported type cast from type <{sourceType.Name}> to type <{targetType.Name}>.");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast, 
                    _context.I18N.GetFStr("InvalidValueTypeCast", sourceType.Name, targetType.Name));
            }
            return _convertors[sourceType.Name].CastValue(targetType, sourceValue);
        }

        private bool IsNeedNoConvert(Type sourceType, ITypeData targetType)
        {
            if (sourceType.Name.Equals(targetType.Name) && sourceType.Namespace.Equals(targetType.Namespace))
            {
                return true;
            }
            Type targetRealType = _context.TypeInvoker.GetType(targetType);
            return ModuleUtils.IsNeedNoConvert(sourceType, targetRealType);
        }

        public object CastValue(Type targetType, object sourceValue)
        {
            if (null == sourceValue)
            {
                _context.LogSession.Print(LogLevel.Warn, _context.SessionId, "Cannot cast null value.");
                return null;
            }
            Type sourceType = sourceValue.GetType();
            if (ModuleUtils.IsNeedNoConvert(sourceType, targetType))
            {
                return sourceValue;
            }
            if (!IsValidValueCast(sourceType, targetType))
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
            else if (_strConvertor.IsValidCastTarget(targetType))
            {
                return _strConvertor.CastValue(targetType, sourceValue);
            }
            else if (_nonValueConvertor.IsNonValueTypeString(targetType, ref sourceValue))
            {
                return _nonValueConvertor.CastConstantValue(targetType, sourceValue);
            }
            throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                _context.I18N.GetFStr("InvalidTypeCast", targetType.Name));
        }

        public object GetDefaultValue(ITypeData type)
        {
            return _convertors.ContainsKey(type.Name) ? _convertors[type.Name].GetDefaultValue() : null;
        }
        
        private bool IsValidValueCast(Type sourceType, ITypeData targetType)
        {
            return _convertors.ContainsKey(sourceType.Name) &&
                   _convertors[sourceType.Name].IsValidCastTarget(targetType);
        }

        public bool IsValidValueCast(Type sourceType, Type targetType)
        {
            return _convertors.ContainsKey(sourceType.Name) &&
                    _convertors[sourceType.Name].IsValidCastTarget(targetType);
        }
    }
}