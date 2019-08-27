using Testflow.Data;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class BoolConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(decimal).Name, sourceValue => (bool)sourceValue ? 1 : 0);
//            ConvertFuncs.Add(typeof(double).Name, sourceValue => sourceValue.ToString());
//            ConvertFuncs.Add(typeof(float).Name, sourceValue => sourceValue.ToString());
            ConvertFuncs.Add(typeof(long).Name, sourceValue => (long)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(ulong).Name, sourceValue => (ulong)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(int).Name, sourceValue => (int)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(uint).Name, sourceValue => (uint)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(short).Name, sourceValue => (short)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(ushort).Name, sourceValue => (ushort)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(char).Name, sourceValue => (char)((bool)sourceValue ? 1 : 0));
            ConvertFuncs.Add(typeof(byte).Name, sourceValue => (byte)((bool)sourceValue ? 1 : 0));
//            ConvertFuncs.Add(typeof(bool).Name, sourceValue => sourceValue.ToString());
            ConvertFuncs.Add(typeof(string).Name, sourceValue => sourceValue.ToString());
        }
    }
}