using Testflow.Data;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class FloatConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => System.Convert.ToDecimal((float)sourceValue));
            ConvertFuncs.Add(typeof(double).Name, sourceValue => System.Convert.ToDouble((float)sourceValue));
//            ConvertFuncs.Add(typeof(float).Name, sourceValue => System.Convert.ToSingle((float)sourceValue));
            ConvertFuncs.Add(typeof(long).Name, sourceValue => System.Convert.ToInt64((float)sourceValue));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => System.Convert.ToUInt64((float)sourceValue));
            ConvertFuncs.Add(typeof(int).Name, sourceValue => System.Convert.ToInt32((float)sourceValue));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => System.Convert.ToUInt32((float)sourceValue));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => System.Convert.ToInt16((float)sourceValue));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => System.Convert.ToUInt16((float)sourceValue));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => System.Convert.ToChar((float)sourceValue));
            ConvertFuncs.Add(typeof(byte).Name, sourceValue => System.Convert.ToByte((float)sourceValue));
            ConvertFuncs.Add(typeof(bool).Name, sourceValue => (float)sourceValue > 0);
            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }
    }
}