using Testflow.Usr;

namespace Testflow.DataMaintainer
{
    public static class ModuleErrorCode
    {
        public const int ConnectDbFailed = 1 | CommonErrorCode.DataMaintainErrorMask;
        public const int DbOperationFailed = 2 | CommonErrorCode.DataMaintainErrorMask;
        public const int DbOperationTimeout = 3 | CommonErrorCode.DataMaintainErrorMask;
    }
}