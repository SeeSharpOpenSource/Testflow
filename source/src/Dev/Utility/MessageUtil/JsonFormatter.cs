using System;
using System.IO;
using System.Messaging;
using System.Text;

namespace MessageQueuing
{
    /// <summary>
    /// Json格式化器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonMessageFormatter<T> : IMessageFormatter
    {
        private Encoding encoding;
        public JsonMessageFormatter()
        {
            this.encoding = Encoding.UTF8;
        }
        public JsonMessageFormatter(Encoding encoding)
        {
            this.encoding = encoding;
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
            return new JsonMessageFormatter<T>(encoding);
        }
        public object Read(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            using (var reader = new StreamReader(message.BodyStream, encoding))
            {
                var json = reader.ReadToEnd();
                return NetJSON.NetJSON.Deserialize<T>(json);
            }
        }
        public void Write(Message message, object obj)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            string json = NetJSON.NetJSON.Serialize(obj);
            message.BodyStream = new MemoryStream(encoding.GetBytes(json));
        }
    }
}