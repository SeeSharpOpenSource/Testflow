using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testflow.ParameterChecker.Convertor
{
    public static class ValueConvertor
    {
        private static Dictionary<string, Func<string, bool>> _convertorHandler;

        static ValueConvertor()
        {

            _convertorHandler = new Dictionary<string, Func<string, bool>>(10)
            {
                {typeof (string).Name, (valueStr) => true},
                {typeof (int).Name, (valueStr) => {int n;  return int.TryParse(valueStr, out n); } },
                {typeof (uint).Name, (valueStr) => {uint n; return uint.TryParse(valueStr, out n); } },
                {typeof (short).Name, (valueStr) => {short n; return short.TryParse(valueStr, out n); } },
                {typeof (ushort).Name, (valueStr) => {ushort n; return ushort.TryParse(valueStr, out n); } },
                {typeof (long).Name, (valueStr) => {long n; return long.TryParse(valueStr, out n); } },
                {typeof (ulong).Name, (valueStr) => {ulong n; return ulong.TryParse(valueStr, out n); } },
                {typeof (byte).Name, (valueStr) => {byte n; return byte.TryParse(valueStr, out n); } },
                {typeof (char).Name, (valueStr) => {char n; return char.TryParse(valueStr, out n); } },
                {typeof (bool).Name, (valueStr) => {bool n; return bool.TryParse(valueStr, out n); } },
                {typeof (DateTime).Name, (valueStr) => {DateTime n; return DateTime.TryParse(valueStr, out n); } },
            };
        }

        public static bool CheckValue(string typeName, string attributeValue)
        {
            return _convertorHandler[typeName].Invoke(attributeValue);
        }
    }
}