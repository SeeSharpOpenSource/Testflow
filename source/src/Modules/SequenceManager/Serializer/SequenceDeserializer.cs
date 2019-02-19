using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Testflow.Common;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Serializer
{
    public class SequenceDeserializer
    {
        #region 反序列化

        public static TestProject LoadTestProject(string filePath, IModuleConfigData envInfo)
        {
            HashSet<Type> typeSet = new HashSet<Type>();
            const string sequenceElementNameSpace = "Testflow.SequenceManager.SequenceElements";
            Type testProjectType = typeof(TestProject);
            Type[] assemblyTypes = Assembly.GetAssembly(testProjectType).GetTypes();
            foreach (Type typeObj in assemblyTypes)
            {
                if (sequenceElementNameSpace.Equals(typeObj.Namespace))
                {
                    typeSet.Add(typeObj);
                }
            }
            typeSet.Remove(testProjectType);
            if (!filePath.EndsWith($".{CommonConst.TestGroupFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(Constants.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(testProjectType, typeSet.ToArray());
                    TestProject testProject = serializer.Deserialize(fileStream) as TestProject;
                    return testProject;
                }
            }
            catch (IOException ex)
            {
                throw new TestflowRuntimeException(Constants.SerializeFailed, ex.Message, ex);
            }
        }

        public static SequenceGroup LoadSequenceGroup(string filePath, IModuleConfigData envInfo)
        {
            HashSet<Type> typeSet = new HashSet<Type>();
            const string sequenceElementNameSpace = "Testflow.SequenceManager.SequenceElements";
            Type sequenceGroupType = typeof(TestProject);
            Type[] assemblyTypes = Assembly.GetAssembly(sequenceGroupType).GetTypes();
            foreach (Type typeObj in assemblyTypes)
            {
                if (sequenceElementNameSpace.Equals(typeObj.Namespace))
                {
                    typeSet.Add(typeObj);
                }
            }
            typeSet.Remove(typeof(TestProject));
            typeSet.Remove(sequenceGroupType);
            if (!filePath.EndsWith($".{CommonConst.SequenceFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(Constants.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(sequenceGroupType, typeSet.ToArray());
                    SequenceGroup testProject = serializer.Deserialize(fileStream) as SequenceGroup;
                    return testProject;
                }
            }
            catch (IOException ex)
            {
                throw new TestflowRuntimeException(Constants.SerializeFailed, ex.Message, ex);
            }
        }

        #endregion


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