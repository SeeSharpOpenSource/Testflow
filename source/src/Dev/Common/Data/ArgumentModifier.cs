namespace Testflow.Data
{
    public enum ArgumentModifier
    {
        /// <summary>
        /// 无修饰符
        /// </summary>
        None = 0,

        /// <summary>
        /// 使用ref修饰
        /// </summary>
        Ref = 1, 

        /// <summary>
        /// 使用out修饰
        /// </summary>
        Out = 2
    }
}