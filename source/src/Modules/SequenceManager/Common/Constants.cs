namespace Testflow.SequenceManager.Common
{
    internal class Constants
    {
        public const int ReturnArgIndex = -1;

        public const int DefaultSequenceSize = 10;

        public const int DefaultArgumentSize = 10;

        public const int DefaultTypeCollectionSize = 100;

        public const int UnverifiedTypeIndex = -1;

        public const int UnverifiedIndex = -3;

        public const int UnverifiedSequenceIndex = -1;

        public const string I18nName = "SequenceMananger";

        public const string BakFileExtension = ".bak";

        public const string TestProjectNameFormat = "TestProject{0}";
        public const string SequenceGroupNameFormat = "SequenceGroup{0}";
        public const string SequenceNameFormat = "Sequence{0}";
        public const string StepNameFormat = "SequenceStep{0}";
        public const string VariableNameFormat = "Variable{0}";

        public const string CopyPostfix = "-Copy";

        public const string ValueTypeName = "Value";

//        public const string CollectionElemName = "Element";

        #region 参数名称

        public const string VersionName = "ModelVersion";

        public const string Encoding = "FileEncoding";

        #endregion

        public const string FuncAssemblyName = "funcdefs";

        public const string FuncNamespace = "Testflow.FunctionDefinitions";

        public const string FuncClassName = "StepFunctions";

        #region 表达式相关

        public const string ArgNameFormat = "ARG{0}";
        public const string ArgNamePattern = "ARG\\d+";
        public const string ExpPlaceHodlerPattern = "EXP\\d+";
        public const string ExpPlaceHodlerFormat = "EXP{0}";
        public const string SingleArgPattern = "^ARG\\d+$";
        public const string SingleExpPattern = "^EXP\\d+$";
        public const string DigitPattern = "^(?:\\d+(?:\\.\\d+)?|0x[0-9a-fA-F]+|\\d+(?:\\.\\d+)?[Ee](?:[\\+-]?\\d+))?$";
        public const string StringPattern = "^(\"|')(.*)\\1$";

        #endregion

    }
}