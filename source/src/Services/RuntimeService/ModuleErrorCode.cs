using Testflow.Usr;


namespace Testflow.RuntimeService
{
    public class ModuleErrorCode
    {
        public const int TestProjectDNE = 1 | CommonErrorCode.RuntimeErrorMask;
        public const int SequenceGroupDNE = 2 | CommonErrorCode.RuntimeErrorMask;
        public const int ServiceNotLoaded = 3 | CommonErrorCode.RuntimeErrorMask;
    }
}
