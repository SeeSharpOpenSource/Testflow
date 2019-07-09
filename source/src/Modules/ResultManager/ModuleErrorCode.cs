using Testflow.Usr;

namespace Testflow.ResultManager
{
    public static class ModuleErrorCode
    {
        public const int InvalidFilePath = 1 | CommonErrorCode.ResultManageErrorMask;
        public const int IOError = 2 | CommonErrorCode.ResultManageErrorMask;
        public const int CustomWriterNonExistent = 3 | CommonErrorCode.ResultManageErrorMask;
    }
}
