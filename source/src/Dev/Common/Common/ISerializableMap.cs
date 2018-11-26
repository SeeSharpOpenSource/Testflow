using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Testflow.Common
{
    /// <summary>
    /// 可序列化的键值集合
    /// </summary>
    /// <typeparam name="TKeyType">键类型</typeparam>
    /// <typeparam name="TValueType">数值类型</typeparam>
    public interface ISerializableMap<TKeyType, TValueType> : IDictionary<TKeyType, TValueType>, ISerializable, IXmlSerializable
    {
         
    }
}