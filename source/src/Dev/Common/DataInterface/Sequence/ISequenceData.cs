using System;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 保存单个测试序列信息的数据结构
    /// </summary>
    public interface ISequenceData: ICloneable
    {
        string Name { get; set; }
        string Description { get; set; }
        IVariableCollection Variables { get; set; }
    }
}