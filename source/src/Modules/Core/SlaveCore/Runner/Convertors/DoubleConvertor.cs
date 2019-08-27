using Testflow.Data;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class DoubleConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => System.Convert.ToDecimal((double)sourceValue));
//            ConvertFuncs.Add(typeof(double).Name, sourceValue => System.Convert.ToDouble((double)sourceValue));
            ConvertFuncs.Add(typeof(float).Name, sourceValue => System.Convert.ToSingle((double)sourceValue));
            ConvertFuncs.Add(typeof(long).Name, sourceValue => System.Convert.ToInt64((double)sourceValue));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => System.Convert.ToUInt64((double)sourceValue));
            ConvertFuncs.Add(typeof(int).Name, sourceValue => System.Convert.ToInt32((double)sourceValue));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => System.Convert.ToUInt32((double)sourceValue));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => System.Convert.ToInt16((double)sourceValue));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => System.Convert.ToUInt16((double)sourceValue));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => System.Convert.ToChar((double)sourceValue));
            ConvertFuncs.Add(typeof(byte).Name, sourceValue => System.Convert.ToByte((double)sourceValue));
            ConvertFuncs.Add(typeof(bool).Name, sourceValue => (double)sourceValue > 0);
            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }
    }
}