using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Testflow.Common;
using Testflow.Logger;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Serializer
{
    public static class XmlReaderHelper
    {
        public static void Read(TestProject testProject)
        {

        }

        public static void Read(SequenceGroup sequenceGroup)
        {

        }

        public static void Read(SequenceGroupParameter parameter)
        {

        }

        private static XmlReader CreateXmlReader(string filePath)
        {
            try
            {
                return XmlReader.Create(filePath);
            }
            catch (IOException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex, ex.Message);
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(SequenceManagerErrorCode.SerializeFailed, ex.Message, ex);
            }
        }

        private static Dictionary<Type, Type> GetTypeMapping()
        {
            Dictionary<Type, Type> typeMapping = new Dictionary<Type, Type>(40);
            return typeMapping;
        }
    }
}