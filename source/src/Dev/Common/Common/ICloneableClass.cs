namespace Testflow.Usr
{
    /// <summary>
    /// 可拷贝接口
    /// </summary>
    public interface ICloneableClass<out TDataType>
    {
        /// <summary>
        /// 复制当前的实例
        /// </summary>
        TDataType Clone();
    }
}