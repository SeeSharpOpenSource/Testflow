using Testflow.Usr;

namespace Testflow.ParameterChecker
{
    public static class ModuleErrorCode
    {
        public const int InvalidParent = 1 | CommonErrorCode.ParamCheckErrorMask;
        public const int VariableNotFound = 2 | CommonErrorCode.ParamCheckErrorMask;
        public const int InvalidType = 3 | CommonErrorCode.ParamCheckErrorMask;
    }
}
