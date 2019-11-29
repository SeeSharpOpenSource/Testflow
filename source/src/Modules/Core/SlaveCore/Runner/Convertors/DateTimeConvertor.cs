using System;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Convertors
{
    internal class DateTimeConvertor : ValueConvertorBase
    {
        protected override void InitializeConvertFuncs()
        {
            ConvertFuncs.Add(typeof(string).Name, sourceValue => ((DateTime)sourceValue).ToString(CommonConst.GlobalTimeFormat));
        }

        public override object GetDefaultValue()
        {
            return DateTime.MinValue;
        }
    }
}