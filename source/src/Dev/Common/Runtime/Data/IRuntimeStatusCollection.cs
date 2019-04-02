using Testflow.Common;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 运行时状态集合
    /// </summary>
    public interface IRuntimeStatusCollection : ISerializableMap<int, IRuntimeStatusInfo>
    {
         
    }
}