using Testflow.Usr;

namespace Testflow.DesigntimeService
{
    public static class ModuleErrorCode
    {
        public const int TestProjectDNE = 1 | CommonErrorCode.DesigntimeErrorMask;
        public const int ComponentDNE = 2 | CommonErrorCode.DesigntimeErrorMask;
        public const int SequenceGroupDNE = 3 | CommonErrorCode.DesigntimeErrorMask;
        public const int SequenceDNE = 4 | CommonErrorCode.DesigntimeErrorMask;
        public const int VariableNotFound = 5 | CommonErrorCode.DesigntimeErrorMask;
        public const int VariableExists = 6 | CommonErrorCode.DesigntimeErrorMask;
        public const int InvalidParent = 7 | CommonErrorCode.DesigntimeErrorMask;
    }
}
