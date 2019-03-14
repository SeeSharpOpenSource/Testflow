using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.EngineCore.Common;

namespace Testflow.EngineCore.Data
{
    public class DebugData : ISerializable
    {
        /// <summary>
        /// 该处的名称为SequenceGroupIndex.SequenceIndex.VariableName组成
        /// </summary>
        public List<string> Names { get; }
        public List<string> Values { get; }
        public List<int> Types { get; }

        public int Count => Names.Count;

        public static string GetVarName(int sequenceGroupIndex, int sequenceIndex, string name)
        {
            return $"{sequenceGroupIndex}.{sequenceIndex}.{name}";
        }

        public DebugData()
        {
            this.Names = new List<string>(Constants.DefaultRuntimeSize);
            this.Values = new List<string>(Constants.DefaultRuntimeSize);
            this.Types = new List<int>(Constants.DefaultRuntimeSize);
        }

        public DebugData(SerializationInfo info, StreamingContext context)
        {
            this.Names = info.GetValue("Names", typeof(List<string>)) as List<string>;
            this.Values = info.GetValue("Values",typeof(List<string>)) as List<string>;
            this.Types = info.GetValue("Types", typeof(List<int>)) as List<int>;
        }

        public void AddData(string name, string value, int typeIndex)
        {
            this.Names.Add(name);
            this.Values.Add(value);
            this.Types.Add(typeIndex);
        }

        public void AddData(int sequenceGroupIndex, int sequenceIndex, string name, string value, int typeIndex)
        {
            this.Names.Add(GetVarName(sequenceGroupIndex, sequenceIndex, name));
            this.Values.Add(value);
            this.Types.Add(typeIndex);
        }

        public void Remove(string name)
        {
            int index = this.Names.IndexOf(name);
            if (-1 == index)
            {
                return;
            }
            this.Names.RemoveAt(index);
            this.Values.RemoveAt(index);
            this.Types.RemoveAt(index);
        }

        public void Remove(int sequenceGroupIndex, int sequenceIndex, string name)
        {
            this.Remove(GetVarName(sequenceGroupIndex, sequenceIndex, name));
        }

        public void Clear()
        {
            this.Names.Clear();
            this.Values.Clear();
            this.Types.Clear();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Names", Names);
            info.AddValue("Values", Values);
            info.AddValue("Types", Types);
        }
    }
}