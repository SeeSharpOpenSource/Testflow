using System;
using Testflow.CoreCommon;
using Testflow.Data;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class StringConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => System.Convert.ToDecimal((string)sourceValue));
            ConvertFuncs.Add(typeof(double).Name, sourceValue => System.Convert.ToDouble((string)sourceValue));
            ConvertFuncs.Add(typeof(float).Name, sourceValue => System.Convert.ToSingle((string)sourceValue));
            ConvertFuncs.Add(typeof(long).Name, sourceValue => System.Convert.ToInt64((string)sourceValue));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => System.Convert.ToUInt64((string)sourceValue));
            ConvertFuncs.Add(typeof(int).Name, sourceValue => System.Convert.ToInt32((string)sourceValue));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => System.Convert.ToUInt32((string)sourceValue));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => System.Convert.ToInt16((string)sourceValue));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => System.Convert.ToUInt16((string)sourceValue));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => System.Convert.ToChar((string)sourceValue));
            ConvertFuncs.Add(typeof (byte).Name, sourceValue => System.Convert.ToByte((string)sourceValue));
            ConvertFuncs.Add(typeof(bool).Name, sourceValue => System.Convert.ToBoolean((string)sourceValue));
            ConvertFuncs.Add(typeof(DateTime).Name, sourceValue =>
            {
                DateTime dateTime;
                bool validCast = DateTime.TryParse((string) sourceValue, out dateTime);
                if (!validCast)
                {
                    throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                        "Illegal string for datetime cast.");
                }
                return dateTime;
            });
//            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }

        public override object GetDefaultValue()
        {
            return "";
        }
    }
}