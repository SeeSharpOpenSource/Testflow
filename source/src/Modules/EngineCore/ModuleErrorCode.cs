using Testflow.Common;

namespace Testflow.EngineCore
{
    public static class ModuleErrorCode
    {
        public const int UnexistSession = 1 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnexistEvent = 2 | CommonErrorCode.EngineCoreErrorMask;
        public const int IncorrectDelegate = 3 | CommonErrorCode.EngineCoreErrorMask;
        public const int IncorrectParamType = 4 | CommonErrorCode.EngineCoreErrorMask;
        public const int EventTimeOut = 5 | CommonErrorCode.EngineCoreErrorMask;
        public const int EventsTooMany = 6 | CommonErrorCode.EngineCoreErrorMask;
    }
}