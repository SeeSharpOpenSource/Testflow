using Testflow.Common;

namespace Testflow.CoreCommon
{
    public static class ModuleErrorCode
    {
        public const int UnexistSession = 1 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnexistEvent = 2 | CommonErrorCode.EngineCoreErrorMask;
        public const int IncorrectDelegate = 3 | CommonErrorCode.EngineCoreErrorMask;
        public const int IncorrectParamType = 4 | CommonErrorCode.EngineCoreErrorMask;
        public const int EventTimeOut = 5 | CommonErrorCode.EngineCoreErrorMask;
        public const int EventsTooMany = 6 | CommonErrorCode.EngineCoreErrorMask;
        public const int SequenceDataError = 7 | CommonErrorCode.EngineCoreErrorMask;
        public const int OperationTimeout = 8 | CommonErrorCode.EngineCoreErrorMask;
        public const int InvalidOperation = 9 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnregisteredMessage = 10 | CommonErrorCode.EngineCoreErrorMask;
        public const int InvalidRuntimeObjectType = 11 | CommonErrorCode.EngineCoreErrorMask;
        public const int InvalidRuntimeInfoName = 11 | CommonErrorCode.EngineCoreErrorMask;

    }
}