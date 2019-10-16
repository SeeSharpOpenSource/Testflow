using System;
using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// Json格式化器
    /// </summary>
    public class JsonMessageFormatter : IMessageFormatter
    {
        private Encoding encoding;
        private MessengerOption _option;
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonMessageFormatter(MessengerOption option, Encoding encoding)
        {
            this.encoding = encoding;
            this._option = option;
            _serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include
            };
        }
        public bool CanRead(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var stream = message.BodyStream;
            return stream != null
            && stream.CanRead
            && stream.Length > 0;
        }
        public object Clone()
        {
            return new JsonMessageFormatter(_option, encoding);
        }
        public object Read(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Type messageType = _option.GetMsgType.Invoke(message.Label);
            using (var reader = new StreamReader(message.BodyStream, encoding))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject(json, messageType, _serializerSettings);
            }
        }
        public void Write(Message message, object obj)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Type messageType = _option.GetMsgType.Invoke(message.Label);
            string json = JsonConvert.SerializeObject(obj, messageType, _serializerSettings);
            message.BodyStream = new MemoryStream(encoding.GetBytes(json));
        }
    }
}