using System;
using System.Collections.Generic;
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

        public static string GetDefaultName(string[] existNames, string nameFormat, int index)
        {
            // 索引号从0开始，命名从1开始，所以要先对index加1
            string defaultName = string.Format(nameFormat, ++index);
            while (existNames.Any(item => item.Equals(defaultName)))
            {
                defaultName = string.Format(nameFormat, ++index);
            }
            return defaultName;
        }

        public static bool IsValidName(string name, params string[] existNames)
        {
            return !string.IsNullOrWhiteSpace(name) && existNames.Contains(name);
        }

        public static string GetHashValue(string hashSource, Encoding encoding)
        {
            string hashValue;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] sourceBytes = encoding.GetBytes(hashSource);
                byte[] valueBytes = sha256.ComputeHash(sourceBytes);
                hashValue = encoding.GetString(valueBytes);
            }
            return hashValue;
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