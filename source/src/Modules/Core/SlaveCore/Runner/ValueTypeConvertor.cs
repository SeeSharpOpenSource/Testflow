using System;
using System.Collections.Generic;
using Testflow.CoreCommon;
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

        public object CastValue(ITypeData sourceType, ITypeData targetType, object sourceValue)
        {
            if (!IsValidCast(sourceType, targetType))
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId, 
                    $"Unsupported type cast from type <{sourceType.Name}> to type <{targetType.Name}>.");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast, 
                    _context.I18N.GetFStr("InvalidValueTypeCast", sourceType.Name, targetType.Name));
            }
            if (null == sourceValue)
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId, "Cannot cast null value.");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("CastNullValue", sourceType.Name));
            }
            return _convertors[sourceType.Name].CastValue(targetType, sourceValue);
        }

        public object CastValueOrDefault(ITypeData sourceType, ITypeData targetType, object sourceValue)
        {
            if (!IsValidCast(sourceType, targetType))
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId,
                    $"Unsupported type cast from type <{sourceType.Name}> to type <{targetType.Name}>.");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("InvalidValueTypeCast", sourceType.Name, targetType.Name));
            }
            return null != sourceValue
                ? _convertors[sourceType.Name].CastValue(targetType, sourceValue)
                : _convertors[sourceType.Name].GetDefaultValue();
        }

        /// <summary>
        /// 字符串转换为值类型
        /// </summary>
        public object CastStrValue(Type targetType, string sourceValue)
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

        private bool IsValidCast(ITypeData sourceType, ITypeData targetType)
        {
            return _convertors.ContainsKey(sourceType.Name) &&
                   _convertors[sourceType.Name].IsValidCastTarget(targetType);
        }
    }
}