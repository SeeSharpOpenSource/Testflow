using System;

namespace Testflow.SequenceManager.Serializer.Convertor
{
    public class EnumConvertor
    {
        public static object ReadData(Type propertyType, string attribute)
        {
            return Enum.Parse(propertyType, attribute);
        }
    }
}