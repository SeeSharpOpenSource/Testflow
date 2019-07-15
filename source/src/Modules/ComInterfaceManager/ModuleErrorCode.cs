using Testflow.Usr;

namespace Testflow.ComInterfaceManager
{
    public static class ModuleErrorCode
    {
        public const int LibraryLoadError = 1 | CommonErrorCode.ComInterfaceErrorMask;
        public const int HighVersion = 2 | CommonErrorCode.ComInterfaceErrorMask;
        public const int LowVersion = 3 | CommonErrorCode.ComInterfaceErrorMask;
        public const int TypeCannotLoad = 4 | CommonErrorCode.ComInterfaceErrorMask;
        public const int PropertyNotFound = 5 | CommonErrorCode.ComInterfaceErrorMask;
        public const int LibraryNotFound = 6 | CommonErrorCode.ComInterfaceErrorMask;
        public const int AssemblyNotLoad = 7 | CommonErrorCode.ComInterfaceErrorMask;
    }
}