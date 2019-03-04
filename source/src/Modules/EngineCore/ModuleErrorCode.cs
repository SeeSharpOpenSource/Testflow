using Testflow.Common;

namespace Testflow.EngineCore
{
    public static class ModuleErrorCode
    {
        public const int UnexistSession = 1 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnexistEvent = 2 | CommonErrorCode.EngineCoreErrorMask;
        public const int IncorrectDelegate = 3 | CommonErrorCode.EngineCoreErrorMask;
        public const int IncorrectParamType = 4 | CommonErrorCode.EngineCoreErrorMask;
        
    }
}