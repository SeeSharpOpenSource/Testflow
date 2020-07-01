using Testflow.Usr;

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

        public const int InvalidMessageReceived = 12 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnavailableLibrary = 13 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnaccessibleType = 14 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnsupportedTypeCast = 15 | CommonErrorCode.EngineCoreErrorMask;
        public const int ForceFailed = 16 | CommonErrorCode.EngineCoreErrorMask;
        public const int CallBackFunctionNameError = 17 | CommonErrorCode.EngineCoreErrorMask;
        public const int RetryFailed = 17 | CommonErrorCode.EngineCoreErrorMask;
        public const int UserForceFailed = 18 | CommonErrorCode.EngineCoreErrorMask;
        public const int UnsupportedPlatform = 19 | CommonErrorCode.EngineCoreErrorMask;
        public const int ExpressionError = 20 | CommonErrorCode.EngineCoreErrorMask;

        public const int RuntimeError = 100 | CommonErrorCode.EngineCoreErrorMask;


    }
}