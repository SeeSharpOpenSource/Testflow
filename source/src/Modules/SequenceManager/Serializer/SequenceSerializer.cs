using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Serializer
{
    internal static class SequenceSerializer
    {
        public static void Serialize(string filePath, TestProject testProject)
        {
            
        }

        public static void Serialize(string filePath, SequenceGroup testProject)
        {

        }

        public static TestProject LoadTestProject(string filePath, IModuleConfigData envInfo)
        {
            
        }

        public static SequenceGroup LoadSequenceGroup(string filePath, IModuleConfigData envInfo)
        {
            
        }

        private static bool IsValidVersion(string version, IModuleConfigData envInfo)
        {
            const char versionDelim = '.';
            string[] versionElem = version.Split(versionDelim);
            string envVersion = envInfo.GetProperty<string>(Constants.VersionName);
            string[] envVersionElem = envVersion.Split(versionDelim);
            int versionElemSize = versionElem.Length <= envVersionElem.Length
                ? versionElem.Length
                : envVersionElem.Length;
            for (int i = 0; i < versionElemSize; i++)
            {
                int versionId = int.Parse(versionElem[i]);
                int envVersionId = int.Parse(envVersionElem[i]);
                if (versionId > envVersionId)
                {
                    return false;
                }
            }
            return envVersionElem.Length >= versionElem.Length || int.Parse(versionElem[versionElemSize]) <= 0;
        }
    }
}