using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testflow.ParameterChecker
{
    public static class WarnCode
    {
        public const int VariableDNE = 1;
        public const int TypeInvalid = 2;
        public const int ParameterDataNotAvailable = 3;
        //无法加载参数类型所需的程序集，一般不会出现这种情况吧
        public const int ParameterTypeAssemblyInvalid = 4;
        //无法从assembly信息找到枚举类，一般不会出现这种情况吧
        public const int EnumClassFault = 5;
        //用户输入的枚举值不在枚举类里
        public const int EnumDNE = 6;
    }
}
