﻿using Testflow.Data;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class LongConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => System.Convert.ToDecimal((long)sourceValue));
            ConvertFuncs.Add(typeof(double).Name, sourceValue => System.Convert.ToDouble((long)sourceValue));
            ConvertFuncs.Add(typeof(float).Name, sourceValue => System.Convert.ToSingle((long)sourceValue));
//            ConvertFuncs.Add(typeof(long).Name, sourceValue => System.Convert.ToInt64((long)sourceValue));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => System.Convert.ToUInt64((long)sourceValue));
            ConvertFuncs.Add(typeof(int).Name, sourceValue => System.Convert.ToInt32((long)sourceValue));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => System.Convert.ToUInt32((long)sourceValue));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => System.Convert.ToInt16((long)sourceValue));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => System.Convert.ToUInt16((long)sourceValue));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => System.Convert.ToChar((long)sourceValue));
            ConvertFuncs.Add(typeof(byte).Name, sourceValue => System.Convert.ToByte((long)sourceValue));
            ConvertFuncs.Add(typeof(bool).Name, sourceValue => (long)sourceValue > 0);
            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }

        public override object GetDefaultValue()
        {
            return (long) 0;
        }
    }
}