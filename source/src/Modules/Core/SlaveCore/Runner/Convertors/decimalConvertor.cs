namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class DecimalConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
//            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => System.Convert.ToDecimal((decimal)sourceValue));
            ConvertFuncs.Add(typeof(double).Name, sourceValue => System.Convert.ToDouble((decimal)sourceValue));
            ConvertFuncs.Add(typeof(float).Name, sourceValue => System.Convert.ToSingle((decimal)sourceValue));
            ConvertFuncs.Add(typeof(long).Name, sourceValue => System.Convert.ToInt64((decimal)sourceValue));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => System.Convert.ToUInt64((decimal)sourceValue));
            ConvertFuncs.Add(typeof(int).Name, sourceValue => System.Convert.ToInt32((decimal)sourceValue));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => System.Convert.ToUInt32((decimal)sourceValue));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => System.Convert.ToInt16((decimal)sourceValue));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => System.Convert.ToUInt16((decimal)sourceValue));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => System.Convert.ToChar((decimal)sourceValue));
            ConvertFuncs.Add(typeof(byte).Name, sourceValue => System.Convert.ToByte((decimal)sourceValue));
            ConvertFuncs.Add(typeof(bool).Name, sourceValue => (decimal)sourceValue > 0);
            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }
    }
}