using System.Runtime.Serialization;

namespace Testflow.CoreCommon.Data
{
    public class PerformanceData : ISerializable
    {
        /// <summary>
        /// 内存占用
        /// </summary>
        public long MemoryUsed { get; set; }

        /// <summary>
        /// 内存占用
        /// </summary>
        public long MemoryAllocated { get; set; }

        /// <summary>
        /// CPU使用事件，单位为ms
        /// </summary>
        public ulong ProcessorTime { get; set; }

        public PerformanceData(long memoryUsed, long memoryAllocated, ulong processorTime)
        {
            this.MemoryUsed = memoryUsed;
            this.MemoryAllocated = memoryAllocated;
            this.ProcessorTime = processorTime;
        }

        public PerformanceData(SerializationInfo info, StreamingContext context)
        {
            this.MemoryUsed = (long) info.GetValue("MemoryUsed", typeof(long));
            this.MemoryAllocated = (long) info.GetValue("MemoryAllocated", typeof(long));
            this.ProcessorTime = (ulong) info.GetValue("ProcessorTime", typeof(ulong));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MemoryUsed", MemoryUsed);
            info.AddValue("MemoryAllocated", MemoryAllocated);
            info.AddValue("ProcessorTime", ProcessorTime);
        }
    }
}