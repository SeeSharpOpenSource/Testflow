
using Testflow.Usr;

namespace Testflow.ParameterChecker
{
    public static class WarnCode
    {
        public const int VariableDNE = 1 | CommonErrorCode.ParamCheckErrorMask;
        public const int TypeInvalid = 2 | CommonErrorCode.ParamCheckErrorMask;
        public const int VariableValueInvalid = 3 | CommonErrorCode.ParamCheckErrorMask;
        public const int ParameterDataNotAvailable = 4 | CommonErrorCode.ParamCheckErrorMask;
        //无法加载参数类型所需的程序集，一般不会出现这种情况吧
        public const int ParameterTypeAssemblyInvalid = 5 | CommonErrorCode.ParamCheckErrorMask;
        //无法从assembly信息找到枚举类，一般不会出现这种情况吧
        public const int EnumClassFault = 6 | CommonErrorCode.ParamCheckErrorMask;
        //用户输入的枚举值不在枚举类里
        public const int EnumDNE = 7 | CommonErrorCode.ParamCheckErrorMask;

        //返回类型为void, 值不为空
        public const int ReturnNotEmpty = 8 | CommonErrorCode.ParamCheckErrorMask;
    }
}
