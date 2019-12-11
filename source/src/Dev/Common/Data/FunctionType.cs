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
        /// Struct类型的构造器
        /// </summary>
        StructConstructor = 1,

        /// <summary>
        /// 实例函数
        /// </summary>
        InstanceFunction = 2,

        /// <summary>
        /// 静态方法
        /// </summary>
        StaticFunction = 3,

        /// <summary>
        /// 实例属性配置方法
        /// </summary>
        InstancePropertySetter = 4,

        /// <summary>
        /// 静态属性配置方法
        /// </summary>
        StaticPropertySetter = 5,

        /// <summary>
        /// 实例属性配置方法
        /// </summary>
        InstanceFieldSetter = 6,

        /// <summary>
        /// 静态属性配置方法
        /// </summary>
        StaticFieldSetter = 7,

        /// <summary>
        /// 断言函数
        /// </summary>
        Assertion = 8,

        /// <summary>
        /// 回调函数
        /// </summary>
        CallBack = 9
    }
}