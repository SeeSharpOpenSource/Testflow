using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.SlaveCore.Common;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner
{
    internal class NonValueTypeConvertor
    {
        private readonly SlaveContext _context;
        private JsonSerializerSettings _settings;

        private const string ArrayDataRegex = @"^\[.*\]$";
        private const string ClassDataRegex = @"^\{.*\}$";

        private Regex _arrayRegex;
        private Regex _classRegex;

        public NonValueTypeConvertor(SlaveContext context)
        {
            this._context = context;
            _settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = CommonConst.GlobalTimeFormat,
                DateParseHandling = DateParseHandling.None
            };
            _arrayRegex = new Regex(ArrayDataRegex);
            _classRegex = new Regex(ClassDataRegex);
        }

        public object CastConstantValue(Type targetType, string objStr)
        {
            if (targetType.IsInterface || targetType.IsAbstract)
            {
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("CastInterface", targetType.Name));
            }
            object castedObject = null;
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            try
            {
                if (targetType.IsArray)
                {
                    string[] datas = JsonConvert.DeserializeObject<string[]>(objStr, _settings);
                    Type elementType = targetType.GetElementType();
                    Array targetInstance = Array.CreateInstance(elementType, datas.Length);
                    for (int i = 0; i < datas.Length; i++)
                    {
                        object elementValue = _context.Convertor.CastConstantValue(elementType, datas[i]);
                        targetInstance.SetValue(elementValue, i);
                    }
                    castedObject = targetInstance;
                }
                // struct
                else if (targetType.IsValueType)
                {
                    object targetInstance = targetType.Assembly.CreateInstance(ModuleUtils.GetTypeFullName(targetType));
                    SetValueToStructOrClass(targetType, objStr, flags, ref targetInstance);
                    castedObject = targetInstance;
                }
                // 暂不考虑List的情况
                else 
                {
                    ConstructorInfo constructor = targetType.GetConstructor(new Type[]{});
                    if (null == constructor)
                    {
                        _context.LogSession.Print(LogLevel.Error, _context.SessionId, 
                            $"Cannot cast string <{objStr}> as target type <{targetType.Name}> has no default constructor.");
                        throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                            _context.I18N.GetFStr("NoDefaultConstructor", targetType.Name));
                    }
                    object targetInstance = constructor.Invoke(new object[] { });
                    SetValueToStructOrClass(targetType, objStr, flags, ref targetInstance);
                    castedObject = targetInstance;
                }
            }
            catch (JsonReaderException ex)
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId, $"Cast value <{objStr}> failed: {ex.Message}");
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("CastValueFailed", targetType.Name), ex);
            }
            return castedObject;
        }

        private void SetValueToStructOrClass(Type targetType, string objStr, BindingFlags flags, ref object targetInstance)
        {
            Dictionary<string, string> datas = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                objStr, _settings);
            foreach (string propertyName in datas.Keys)
            {
                PropertyInfo propertyInfo = targetType.GetProperty(propertyName, flags);
                if (null != propertyInfo)
                {
                    object propertyValue = _context.Convertor.CastConstantValue(propertyInfo.PropertyType,
                        datas[propertyName]);
                    propertyInfo.SetValue(targetInstance, propertyValue);
                    continue;
                }
                FieldInfo fieldInfo = targetType.GetField(propertyName, flags);
                if (null != fieldInfo)
                {
                    object propertyValue = _context.Convertor.CastConstantValue(fieldInfo.FieldType,
                        datas[propertyName]);
                    fieldInfo.SetValue(targetInstance, propertyValue);
                    continue;
                }
            }
        }

        public bool IsNonValueTypeString(ref string valueString)
        {
            return _arrayRegex.IsMatch(valueString) || _classRegex.IsMatch(valueString);
        }
    }
}