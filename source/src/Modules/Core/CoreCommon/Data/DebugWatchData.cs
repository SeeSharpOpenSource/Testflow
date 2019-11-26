using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;

namespace Testflow.CoreCommon.Data
{
    [Serializable]
    public class DebugWatchData : ISerializable
    {
        /// <summary>
        /// 该处的名称为SequenceGroupIndex.SequenceIndex.VariableName组成
        /// </summary>
        public List<string> Names { get; set; }
        public List<string> Values { get; set; }

        public int Count => Names.Count;

        public static string GetVarName(int sequenceGroupIndex, int sequenceIndex, string name)
        {
            return $"{sequenceGroupIndex}.{sequenceIndex}.{name}";
        }

        public DebugWatchData()
        {
            this.Names = new List<string>(CoreConstants.DefaultRuntimeSize);
            this.Values = new List<string>(CoreConstants.DefaultRuntimeSize);
        }

        public DebugWatchData(SerializationInfo info, StreamingContext context)
        {
            this.Names = info.GetValue("Names", typeof(List<string>)) as List<string>;
            this.Values = info.GetValue("Values",typeof(List<string>)) as List<string>;
        }

        public void AddData(string name, string value, int typeIndex)
        {
            this.Names.Add(name);
            this.Values.Add(value);
        }

        public void AddData(int sequenceGroupIndex, int sequenceIndex, string name, string value, int typeIndex)
        {
            this.Names.Add(GetVarName(sequenceGroupIndex, sequenceIndex, name));
            this.Values.Add(value);
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
        }

        public void Remove(int sequenceGroupIndex, int sequenceIndex, string name)
        {
            this.Remove(GetVarName(sequenceGroupIndex, sequenceIndex, name));
        }

        public void Clear()
        {
            this.Names.Clear();
            this.Values.Clear();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Names", Names);
            info.AddValue("Values", Values);
        }
    }
}