using System;
using System.Collections.Generic;

namespace Testflow.Utility.Collections.Convertor
{
    internal static class ValueConvertor
    {
        private static Dictionary<string, Func<string, object>> _convertorHandler;

        static ValueConvertor()
        {
            _convertorHandler = new Dictionary<string, Func<string, object>>(10)
            {
                {typeof (string).Name, (valueStr) => valueStr},
                {typeof (int).Name, (valueStr) => int.Parse(valueStr)},
                {typeof (uint).Name, (valueStr) => uint.Parse(valueStr)},
                {typeof (short).Name, (valueStr) => short.Parse(valueStr)},
                {typeof (ushort).Name, (valueStr) => ushort.Parse(valueStr)},
                {typeof (long).Name, (valueStr) => long.Parse(valueStr)},
                {typeof (ulong).Name, (valueStr) => ulong.Parse(valueStr)},
                {typeof (byte).Name, (valueStr) => byte.Parse(valueStr)},
                {typeof (char).Name, (valueStr) => char.Parse(valueStr)},
                {typeof (bool).Name, (valueStr) => bool.Parse(valueStr) },
                {typeof (DateTime).Name, (valueStr) => DateTime.Parse(valueStr)}
            };
        }

        public static object ReadData(Type dataType, string attributeValue)
        {
            return _convertorHandler[dataType.Name].Invoke(attributeValue);
        }
    }
}