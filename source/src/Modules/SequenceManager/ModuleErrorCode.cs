using Testflow.Usr;

namespace Testflow.SequenceManager
{
    public static class ModuleErrorCode
    {
        public const int SerializeFailed = 1 | CommonErrorCode.SequenceManageErrorMask;

        public const int DeSerializeFailed = 2 | CommonErrorCode.SequenceManageErrorMask;

        public const int InvalidFileType = 3 | CommonErrorCode.SequenceManageErrorMask;

        public const int UnmatchedParameter = 4 | CommonErrorCode.SequenceManageErrorMask;

        public const int InvalidModelVersion = 5 | CommonErrorCode.SequenceManageErrorMask;

        public const int UnmatchedFileHash = 6 | CommonErrorCode.SequenceManageErrorMask;

        public const int TypeDataError = 7 | CommonErrorCode.SequenceManageErrorMask;

        public const int VariableError = 8 | CommonErrorCode.SequenceManageErrorMask;

        public const int ExpressionError = 9 | CommonErrorCode.SequenceManageErrorMask;
    }
}