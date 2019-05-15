using Testflow.Usr;

namespace Testflow.Utility
{
    /// <summary>
    /// 模块异常码定义
    /// </summary>
    public class ModuleErrorCode
    {
        /// <summary>
        /// 国际化模块运行时异常
        /// </summary>
        public const int I18nRuntimeError = 1 | CommonErrorCode.UtilityErrorMask;
    }
}