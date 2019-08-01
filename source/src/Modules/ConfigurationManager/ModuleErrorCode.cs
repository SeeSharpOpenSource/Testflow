using Testflow.Usr;

namespace Testflow.ConfigurationManager
{
    public static class ModuleErrorCode
    {
        public const int InvalidTestHome = 1 | CommonErrorCode.ConfigureErrorMask;
        public const int ConfigDataError = 2 | CommonErrorCode.ConfigureErrorMask;
    }
}