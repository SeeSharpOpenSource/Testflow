using Testflow.Data;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class IntConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => System.Convert.ToDecimal((int)sourceValue));
            ConvertFuncs.Add(typeof(double).Name, sourceValue => System.Convert.ToDouble((int)sourceValue));
            ConvertFuncs.Add(typeof(float).Name, sourceValue => System.Convert.ToSingle((int)sourceValue));
            ConvertFuncs.Add(typeof(long).Name, sourceValue => System.Convert.ToInt64((int)sourceValue));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => System.Convert.ToUInt64((int)sourceValue));
//            ConvertFuncs.Add(typeof(int).Name, sourceValue => System.Convert.ToInt32((int)sourceValue));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => System.Convert.ToUInt32((int)sourceValue));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => System.Convert.ToInt16((int)sourceValue));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => System.Convert.ToUInt16((int)sourceValue));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => System.Convert.ToChar((int)sourceValue));
            ConvertFuncs.Add(typeof(byte).Name, sourceValue => System.Convert.ToByte((int)sourceValue));
            ConvertFuncs.Add(typeof(bool).Name, sourceValue => (int)sourceValue > 0);
            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }
    }
}