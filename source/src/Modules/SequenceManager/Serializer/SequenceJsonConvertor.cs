using System;
using Newtonsoft.Json;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.Serializer
{
    internal class SequenceJsonConvertor : JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return ReferenceEquals(objectType, typeof (SequenceGroup)) ||
                   ReferenceEquals(objectType, typeof (TestProject));
        }
    }
}
