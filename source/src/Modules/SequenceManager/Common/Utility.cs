using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.Common
{
    internal static class Utility
    {
        public static void CloneDataCollection<TDataType>(IList<TDataType> source, IList<TDataType> target)
            where TDataType : ISequenceDataContainer
        {
            target.Clear();
            foreach (TDataType data in source)
            {
                target.Add((TDataType) data.Clone());
            }
        }

        public static void CloneFlowCollection<TDataType>(IList<TDataType> source, IList<TDataType> target)
            where TDataType : ISequenceFlowContainer
        {
            target.Clear();
            foreach (TDataType data in source)
            {
                target.Add((TDataType) data.Clone());
            }
        }

        public static void CloneCollection<TDataType>(IList<TDataType> source, IList<TDataType> target)
            where TDataType : ICloneableClass<TDataType>
        {
            target.Clear();
            foreach (TDataType data in source)
            {
                target.Add(data.Clone());
            }
        }
        
        public static bool IsValidName(string name, params string[] existNames)
        {
            return !string.IsNullOrWhiteSpace(name) && !existNames.Contains(name);
        }

        public static bool IsValidPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
            if (Directory.Exists(filePath))
            {
                return true;
            }
            int pathIndex = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            return Directory.Exists(filePath.Substring(0, pathIndex));
        }

        public static string GetHashValue(string hashSource, Encoding encoding)
        {
            StringBuilder hashValue = new StringBuilder(256*2/8);
            char[] decimalStrs = new char[] {'0', '1', '2', '3', '4', '5', '6', '7',
                '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', };

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] sourceBytes = encoding.GetBytes(hashSource);
                byte[] valueBytes = sha256.ComputeHash(sourceBytes);
                foreach (byte byteData in valueBytes)
                {
                    hashValue.Append(decimalStrs[byteData >> 4]).Append(decimalStrs[byteData & 0x0F]);
                }
            }
            return hashValue.ToString();
        }

        public static string GetHostInfo()
        {
            string hostName = Environment.MachineName;
            string systemVersion = Environment.OSVersion.VersionString;
            string mac = string.Empty;
            using (ManagementObjectSearcher query =
                new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration"))
            {
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
                    if (mo["IPEnabled"].ToString() == "True")
                        mac = mo["MacAddress"].ToString();
                }
            }
            return $"{hostName}/{systemVersion}/{mac}";
        }

        private const string IndexPropertName = "Index";

        public static bool AddAndRefreshIndex<TDataType>(IList<TDataType> collection, TDataType item)
        {
            if (collection.Contains(item))
            {
                return false;
            }
            int index = collection.Count;
            collection.Add(item);
            PropertyInfo propertyInfo = item.GetType()
                .GetProperty(IndexPropertName, BindingFlags.Instance | BindingFlags.Public);
            propertyInfo?.SetValue(item, index);
            return true;
        }

        public static bool InsertAndRefreshIndex<TDataType>(IList<TDataType> collection, TDataType item, int index)
        {
            if (collection.Count <= index)
            {
                return false;
            }
            collection.Insert(index, item);
            PropertyInfo propertyInfo = item.GetType()
                .GetProperty(IndexPropertName, BindingFlags.Instance | BindingFlags.Public);
            for (int i = index; i < collection.Count; i++)
            {
                propertyInfo?.SetValue(collection[index], index);
            }
            return true;
        }

        public static bool RemoveAndRefreshIndex<TDataType>(IList<TDataType> collection, TDataType item)
        {
            if (!collection.Contains(item))
            {
                return false;
            }
            int index = collection.IndexOf(item);
            collection.Remove(item);
            PropertyInfo propertyInfo = item.GetType()
                .GetProperty(IndexPropertName, BindingFlags.Instance | BindingFlags.Public);
            for (int i = index; i < collection.Count; i++)
            {
                propertyInfo?.SetValue(collection[index], index);
                index++;
            }
            return true;
        }

        public static bool RemoveAtAndRefreshIndex<TDataType>(IList<TDataType> collection, int index)
        {
            if (collection.Count <= index)
            {
                return false;
            }
            TDataType item = collection[index];
            collection.RemoveAt(index);
            PropertyInfo propertyInfo = item.GetType()
                .GetProperty(IndexPropertName, BindingFlags.Instance | BindingFlags.Public);
            for (int i = index; i < collection.Count; i++)
            {
                propertyInfo?.SetValue(collection[index], index);
                index++;
            }
            return true;
        }

        public static void SetElementName(ISequenceFlowContainer self, IEnumerable<ISequenceFlowContainer> existItems)
        {
            string[] existNames = new string[0];
            if (null != existItems && null != self.Parent)
            {
                existNames = (from element in existItems
                              where !ReferenceEquals(element, self)
                              select element.Name).ToArray();
            }
            if (!IsValidName(self.Name, existNames))
            {
                string nameFormat = $"{self.GetType().Name}{{0}}";
                int index = 0;
                // 索引号从0开始，命名从1开始，所以要先对index加1
                string defaultName = string.Format(nameFormat, ++index);
                while (existNames.Any(item => item.Equals(defaultName)))
                {
                    defaultName = string.Format(nameFormat, ++index);
                }
                self.Name = defaultName;
            }
        }

        public static void SetElementName<TDataType>(TDataType self, IEnumerable<TDataType> existItems)
        {
            const string name = "Name";
            string[] existNames = new string[0];
            PropertyInfo nameProperty = self.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (null == nameProperty)
            {
                return;
            }
            if (null != existItems)
            {
                existNames = (from element in existItems
                              where !ReferenceEquals(element, self)
                              select nameProperty.GetValue(element).ToString()).ToArray();
            }
            if (!IsValidName(nameProperty.GetValue(self).ToString(), existNames))
            {
                string nameFormat = $"{self.GetType().Name}{{0}}";
                int index = 0;
                // 索引号从0开始，命名从1开始，所以要先对index加1
                string defaultName = string.Format(nameFormat, ++index);
                while (existNames.Any(item => item.Equals(defaultName)))
                {
                    defaultName = string.Format(nameFormat, ++index);
                }
                nameProperty.SetValue(self, defaultName);
            }
        }

        public static string GetParameterFilePath(string sequenceFilePath)
        {
            const char fileExtensionDelim = '.';
            int delimIndex = sequenceFilePath.LastIndexOf(fileExtensionDelim);
            return sequenceFilePath.Substring(0, delimIndex + 1) + CommonConst.SequenceDataFileExtension;
        }

        public static string GetSequenceGroupPath(string testProjectFilePath, int index)
        {
            int delimIndex = testProjectFilePath.LastIndexOf(Path.DirectorySeparatorChar);
            string fileDirectory = testProjectFilePath.Substring(0, delimIndex + 1);
            string sequenceGroupName = string.Format(Constants.SequenceGroupNameFormat, index + 1);
            return $"{fileDirectory}{sequenceGroupName}{Path.DirectorySeparatorChar}{sequenceGroupName}.{CommonConst.SequenceFileExtension}";
        }

        public static string GetSequenceGroupDirectory(string testProjectFilePath)
        {
            int delimIndex = testProjectFilePath.LastIndexOf(Path.DirectorySeparatorChar);
            string fileDirectory = testProjectFilePath.Substring(0, delimIndex + 1);
            return fileDirectory;
        }

        public static void RefreshTypeIndex(ITestProject testProject)
        {
            foreach (IVariable variable in testProject.Variables)
            {
                Variable variableObj = (variable as Variable);
                variableObj.TypeIndex = testProject.TypeDatas.IndexOf(variableObj.Type);
            }
            RefreshTypeIndex(testProject.SetUp, testProject.TypeDatas);
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                RefreshTypeIndex(sequenceGroup);
            }
            RefreshTypeIndex(testProject.TearDown, testProject.TypeDatas);
        }

        public static void RefreshTypeIndex(ISequenceGroup sequenceGroup)
        {
            foreach (IArgument argument in sequenceGroup.Arguments)
            {
                RefreshTypeIndex(argument, sequenceGroup.TypeDatas);
            }
            RefreshTypeIndex(sequenceGroup.SetUp, sequenceGroup.TypeDatas);
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                RefreshTypeIndex(sequence, sequenceGroup.TypeDatas);
            }
            RefreshTypeIndex(sequenceGroup.TearDown, sequenceGroup.TypeDatas);
        }

        private static void RefreshTypeIndex(ISequence sequence, ITypeDataCollection typeDataCollection)
        {
            foreach (IVariable variable in sequence.Variables)
            {
                Variable variableObj = (variable as Variable);
                variableObj.TypeIndex = typeDataCollection.IndexOf(variableObj.Type);
            }
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                RefreshTypeIndex(sequenceStep, typeDataCollection);
            }
        }

        private static void RefreshTypeIndex(ISequenceStep sequenceStep, ITypeDataCollection typeDataCollection)
        {
            if (sequenceStep.HasSubSteps)
            {
                foreach (ISequenceStep subStep in sequenceStep.SubSteps)
                {
                    RefreshTypeIndex(subStep, typeDataCollection);
                }
            }
            else
            {
                FunctionData functionData = sequenceStep.Function as FunctionData;
                functionData.ClassTypeIndex = typeDataCollection.IndexOf(functionData.ClassType);
                foreach (IArgument argument in functionData.ParameterType)
                {
                    RefreshTypeIndex(argument, typeDataCollection);
                }
                RefreshTypeIndex(functionData.ReturnType, typeDataCollection);
            }
        }

        private static void RefreshTypeIndex(IArgument argument, ITypeDataCollection typeDataCollection)
        {
            Argument argumentObj = argument as Argument;
            argumentObj.TypeIndex = typeDataCollection.IndexOf(argumentObj.Type);
        }
    }
}