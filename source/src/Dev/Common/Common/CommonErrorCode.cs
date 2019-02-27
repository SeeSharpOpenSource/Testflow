namespace Testflow.Common
{
    /// <summary>
    /// Testflow公共异常码定义
    /// </summary>
    public static class CommonErrorCode
    {
        /// <summary>
        /// 内部异常
        /// </summary>
        public const int InternalError = 1 | CommonErrorMask;

        /// <summary>
        /// 非法操作
        /// </summary>
        public const int InvalidOperation = 2 | CommonErrorMask;

        /// <summary>
        /// 国际化模块运行时异常
        /// </summary>
        public const int I18nRuntimeError = 3 | CommonErrorMask;

        #region 各模块异常码掩码定义

        /// <summary>
        /// 公共模块异常码掩码
        /// </summary>
        public const int CommonErrorMask = 0x0000;

        /// <summary>
        /// 日志模块异常码掩码
        /// </summary>
        public const int LogErrorMask = 0x0100;

        /// <summary>
        /// 工具模块异常码掩码
        /// </summary>
        public const int UtilityErrorMask = 0x0200;

        /// <summary>
        /// 接口加载模块异常码掩码
        /// </summary>
        public const int ComInterfaceErrorMask = 0x0300;

        /// <summary>
        /// 参数检查模块异常码掩码
        /// </summary>
        public const int ParamCheckErrorMask = 0x0400;

        /// <summary>
        /// 序列管理模块异常码掩码
        /// </summary>
        public const int SequenceManageErrorMask = 0x0500;

        /// <summary>
        /// 配置模块异常码掩码
        /// </summary>
        public const int ConfigureErrorMask = 0x0600;

        /// <summary>
        /// 运行引擎模块异常码掩码
        /// </summary>
        public const int EngineCoreErrorMask = 0x1000;

        /// <summary>
        /// 数据维护模块异常码掩码
        /// </summary>
        public const int DataMaintainErrorMask = 0x2000;

        /// <summary>
        /// 结果管理模块异常码掩码
        /// </summary>
        public const int ResultManageErrorMask = 0x3000;

        /// <summary>
        /// 设计时模块异常码掩码
        /// </summary>
        public const int DesigntimeErrorMask = 0x4000;

        /// <summary>
        /// 运行时模块异常码掩码
        /// </summary>
        public const int RuntimeErrorMask = 0x5000;

        #endregion

    }
}