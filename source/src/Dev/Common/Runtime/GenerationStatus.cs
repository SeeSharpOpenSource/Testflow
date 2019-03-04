namespace Testflow.Runtime
{
    /// <summary>
    /// 测试生成状态
    /// </summary>
    public enum GenerationStatus
    {
         /// <summary>
         /// 待生成
         /// </summary>
         Idle = 0,

         /// <summary>
         /// 正在生成的过程
         /// </summary>
         InProgress = 1,

         /// <summary>
         /// 生成成功
         /// </summary>
         Success = 2,

         /// <summary>
         /// 生成失败
         /// </summary>
         Failed = 3
    }
}