using System;

namespace Testflow.Utility.Collections.Convertor
{
    internal class EnumConvertor
    {
        public static object ReadData(Type propertyType, string attribute)
        {
            return Enum.Parse(propertyType, attribute);
        }
    }
}