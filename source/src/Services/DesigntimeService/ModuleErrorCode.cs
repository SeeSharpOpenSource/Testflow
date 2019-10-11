using Testflow.Usr;

namespace Testflow.DesigntimeService
{
    public static class ModuleErrorCode
    {
        public const int InvalidEditOperation = 0 | CommonErrorCode.DesigntimeErrorMask;
        public const int DuplicateDefinition = 1 | CommonErrorCode.DesigntimeErrorMask;
        public const int TargetNotExist = 2 | CommonErrorCode.DesigntimeErrorMask;
    }
}
