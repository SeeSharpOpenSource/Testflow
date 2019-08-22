using Testflow.Usr;

namespace Testflow.DesigntimeService
{
    public static class ModuleErrorCode
    {
        public const int TestProjectNotFound = 1 | CommonErrorCode.DesigntimeErrorMask;
        public const int ComponentNotFound = 2 | CommonErrorCode.DesigntimeErrorMask;
        public const int SequenceGroupNotFound = 3 | CommonErrorCode.DesigntimeErrorMask;
        public const int SequenceNotFound = 4 | CommonErrorCode.DesigntimeErrorMask;
        public const int StepNotFound = 5 | CommonErrorCode.DesigntimeErrorMask;
        public const int VariableNotFound = 6 | CommonErrorCode.DesigntimeErrorMask;
        public const int VariableExists = 7 | CommonErrorCode.DesigntimeErrorMask;
        public const int ParameterNotFound = 8 | CommonErrorCode.DesigntimeErrorMask;
        public const int InvalidParent = 9 | CommonErrorCode.DesigntimeErrorMask;
    }
}
