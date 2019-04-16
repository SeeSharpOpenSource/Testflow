namespace Testflow.Data
{
    /// <summary>
    /// 方法类型
    /// </summary>
    public enum FunctionType
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        Constructor = 0,

        /// <summary>
        /// 实例函数
        /// </summary>
        InstanceFunction = 1,

        /// <summary>
        /// 静态方法
        /// </summary>
        StaticFunction = 2,

        /// <summary>
        /// 断言函数
        /// </summary>
        Assertion = 3,

        /// <summary>
        /// 回调函数
        /// </summary>
        CallBack = 4
    }
}