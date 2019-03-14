using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.Common
{
    internal static class ModuleUtils
    {
        #region Collection Utils

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

        #endregion

        public static bool IsValidName(string name, params string[] existNames)
        {
            return !string.IsNullOrWhiteSpace(name) && !existNames.Contains(name);
        }

        public static bool IsValidFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
            if (File.Exists(filePath))
            {
                return true;
            }
            int pathIndex = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            return Directory.Exists(filePath.Substring(0, pathIndex)) && IsFile(filePath);
        }

        #region Hash Calculation

        public static string GetHashValue(string hashSource, Encoding encoding)
        {
            StringBuilder hashValue = new StringBuilder(256*2/8);
            char[] decimalStrs = new char[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7',
                '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
            };

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

        #endregion

        #region Collection Operation

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

        #endregion

        #region Element Name Generation

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
            if (null == nameProperty.GetValue(self) || !IsValidName(nameProperty.GetValue(self).ToString(), existNames))
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

        #endregion

        #region Type Related
        
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

        // 获取集合类中接口的原始接口类型，例如通过IList<string>的类型获取string类型
        public static Type GetRawGenericElementType(Type collectionType)
        {
            Type[] genericType = null;
            while (null == genericType || 0 == genericType.Length)
            {
                collectionType = collectionType.GetInterfaces()[0];
                genericType = collectionType.GetGenericArguments();
            }
            return genericType[0];
        }

        #endregion

        #region Path Operation

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
            return
                $"{fileDirectory}{sequenceGroupName}{Path.DirectorySeparatorChar}{sequenceGroupName}.{CommonConst.SequenceFileExtension}";
        }

        public static string GetSequenceGroupDirectory(string testProjectFilePath)
        {
            int delimIndex = testProjectFilePath.LastIndexOf(Path.DirectorySeparatorChar);
            string fileDirectory = testProjectFilePath.Substring(0, delimIndex + 1);
            return fileDirectory;
        }

        public static string GetAbsolutePath(string path, string referencePath)
        {
            // 取上级目录的字符串
            const string parentDirStr = "..";
            char dirDelim = Path.DirectorySeparatorChar;
            string regexFormat = dirDelim.Equals('\\')
                ? $"^(([a-zA-z]:)?{dirDelim}{dirDelim})"
                : $"^(([a-zA-z]:)?{dirDelim})";
            // 绝对路径匹配模式，如果匹配则path已经是绝对路径
            Regex regex = new Regex(regexFormat);
            if (0 != regex.Matches(path).Count)
            {
                return path;
            }
            if (0 == regex.Matches(referencePath).Count)
            {
                referencePath = GetAbsolutePath(referencePath, Directory.GetCurrentDirectory());
            }
            //如果不是已分隔符结尾说明相对路径是文件，需要切割为完整路径
            if (IsFile(referencePath))
            {
                int index = referencePath.LastIndexOf(dirDelim);
                referencePath = referencePath.Substring(0, index);
            }
            //如果目录结尾不包含文件夹分隔符则手动添加
            if (referencePath.EndsWith(dirDelim.ToString()))
            {
                referencePath = referencePath.Remove(referencePath.Length - 1);
            }
            if (!path.StartsWith($"{parentDirStr}{dirDelim}"))
            {
                return referencePath + dirDelim + path;
            }

            bool isFilePath = IsFile(path);
            string fileName = string.Empty;
            if (isFilePath)
            {
                int index = path.LastIndexOf(dirDelim);
                fileName = path.Substring(index + 1, path.Length - index - 1);
                path = path.Substring(0, index);
            }
            // 如果最后一个是文件夹分隔符，则删除分隔符
            if (path.EndsWith(dirDelim.ToString()))
            {
                path = path.Remove(referencePath.Length - 1, 1);
            }
            // 取上级目录的次数
            int upLevelCount = 0;
            StringBuilder absolutePath = new StringBuilder(100);
            string[] pathElems = path.Split(dirDelim);
            string[] refPathElems = referencePath.Split(dirDelim);
            for (int i = 0; i < pathElems.Length; i++)
            {
                if (!parentDirStr.Equals(pathElems[i]))
                {
                    upLevelCount = i;
                    break;
                }
            }
            for (int i = 0; i < refPathElems.Length - upLevelCount; i++)
            {
                absolutePath.Append(refPathElems[i]).Append(dirDelim);
            }
            for (int i = upLevelCount; i < pathElems.Length; i++)
            {
                absolutePath.Append(pathElems[i]).Append(dirDelim);
            }
            // 如果PathElemes不是文件夹，删除最后一个分隔符
//            if (pathElems.Length > 0 && !path.EndsWith(dirDelim.ToString()))
            if (pathElems.Length > 0 && absolutePath.Length > 0)
            {
                absolutePath.Remove(absolutePath.Length - 1, 1);
            }
            if (isFilePath)
            {
                absolutePath.Append(dirDelim).Append(fileName);
            }
            return absolutePath.ToString();
        }

        public static string GetRelativePath(string path, string referencePath)
        {
            // 取上级目录的字符串
            const string parentDirStr = "..";
            char dirDelim = Path.DirectorySeparatorChar;
            // 绝对路径匹配模式，如果匹配则path已经是绝对路径
            string regexFormat = dirDelim.Equals('\\')
                ? $"^(([a-zA-z]:)?{dirDelim}{dirDelim})"
                : $"^(([a-zA-z]:)?{dirDelim})";
            Regex regex = new Regex(regexFormat);
            if (0 == regex.Matches(referencePath).Count)
            {
                referencePath = GetAbsolutePath(referencePath, Directory.GetCurrentDirectory());
            }
            //如果不是已分隔符结尾说明相对路径是文件，需要切割为完整路径
            if (IsFile(referencePath))
            {
                int index = referencePath.LastIndexOf(dirDelim);
                referencePath = referencePath.Substring(0, index);
            }
            // 如果最后一个是文件夹分隔符，则删除分隔符
            if (referencePath.EndsWith(dirDelim.ToString()))
            {
                referencePath = referencePath.Remove(referencePath.Length - 1, 1);
            }
            bool isFilePath = IsFile(path);
            string fileName = string.Empty;
            if (isFilePath)
            {
                int index = path.LastIndexOf(dirDelim);
                fileName = path.Substring(index + 1, path.Length - index - 1);
                path = path.Substring(0, index);
            }
            // 如果最后一个是文件夹分隔符，则删除分隔符
            if (path.EndsWith(dirDelim.ToString()))
            {
                path = path.Remove(referencePath.Length - 1, 1);
            }

            //path也是绝对路径
            string[] pathElems = path.Split(dirDelim);
            string[] refPathElems = referencePath.Split(dirDelim);
            StringBuilder relativePath = new StringBuilder(100);
            int checkLength = pathElems.Length > refPathElems.Length ? refPathElems.Length : pathElems.Length;
            int diffIndex = checkLength;
            for (int i = 0; i < checkLength; i++)
            {
                if (!pathElems[i].Equals(refPathElems[i]))
                {
                    diffIndex = i;
                    break;
                }
            }
            if (0 == diffIndex)
            {
                return path;
            }
            for (int refIndex = diffIndex; refIndex < refPathElems.Length; refIndex++)
            {
                relativePath.Append(parentDirStr).Append(dirDelim);
            }
            for (int pathIndex = diffIndex; pathIndex < pathElems.Length; pathIndex++)
            {
                relativePath.Append(pathElems[pathIndex]).Append(dirDelim);
            }
            // 删除最后一个分隔符
//            if (pathElems.Length > 0 && !path.EndsWith(dirDelim.ToString()))
            if (pathElems.Length > 0 && relativePath.Length > 0)
            {
                relativePath.Remove(relativePath.Length - 1, 1);
            }
            if (isFilePath)
            {
                if (relativePath.Length > 0)
                {
                    relativePath.Append(dirDelim);
                }
                relativePath.Append(fileName);
            }
            return relativePath.ToString();
        }

        public static bool IsFile(string path)
        {
            return path.Contains(".") || File.Exists(path);
        }

        public static bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        #endregion

        #region Serialization

        public static void FillSerializationInfo(SerializationInfo info, object obj)
        {
            Type objType = obj.GetType();
            Assembly assembly = Assembly.GetAssembly(typeof (ModuleUtils));
            info.AssemblyName = assembly.GetName().Name;
            info.FullTypeName = $"{objType.Namespace}.{objType.Name}";

            foreach (PropertyInfo propertyInfo in objType.GetProperties())
            {
                RuntimeSerializeIgnoreAttribute ignoreAttribute =
                    propertyInfo.GetCustomAttribute<RuntimeSerializeIgnoreAttribute>();
                if (null != ignoreAttribute && ignoreAttribute.Ignore)
                {
                    continue;
                }
                object value = propertyInfo.GetValue(obj);
                if (null == value)
                {
                    continue;
                }
                Type propertyType = propertyInfo.PropertyType;
                // 如果是类类型且不为null，则获取真实的类型
                if (propertyType.IsClass)
                {
                    propertyType = value.GetType();
                }
                info.AddValue(propertyInfo.Name, value, propertyType);
            }
        }

        public static void FillDeserializationInfo(SerializationInfo info, object obj, Type type)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                RuntimeSerializeIgnoreAttribute ignoreAttribute =
                    propertyInfo.GetCustomAttribute<RuntimeSerializeIgnoreAttribute>();
                if (null != ignoreAttribute && ignoreAttribute.Ignore)
                {
                    continue;
                }
                Type propertyType = propertyInfo.PropertyType;
                // 如果包含RuntimeType属性则说明该属性使用了某个指定的实现类型而非当前属性的类型
                RuntimeTypeAttribute runtimeType;
                if (null != (runtimeType = propertyInfo.GetCustomAttribute<RuntimeTypeAttribute>()))
                {
                    propertyType = runtimeType.RealType;
                }

                //如果包含该属性说明是集合类型，需要特殊处理
                GenericCollectionAttribute genericAttribute = propertyType.GetCustomAttribute<GenericCollectionAttribute>();

                if (null == genericAttribute)
                {
                    object propertyValue = info.GetValue(propertyInfo.Name, propertyType);
                    if (null != propertyValue)
                    {
                        propertyInfo.SetValue(obj, propertyValue);
                    }
                }
                else
                {
                    Type elementType = GetRawGenericElementType(propertyType);
                    if (null == elementType)
                    {
                        throw new InvalidOperationException();
                    }
                    const string addMethodName = "Add";
                    
                    MethodInfo addMethod = propertyType.GetMethod(addMethodName, BindingFlags.Instance | BindingFlags.Public, null,
                            CallingConventions.Standard, new Type[] { elementType }, new ParameterModifier[0]);
                    // 构造类型为List<RealType>的临时集合去获取json的值
                    Type typeCollection = typeof(List<>);
                    Type tmpCollectionType = typeCollection.MakeGenericType(genericAttribute.GenericType);
                    object tmpCollection = info.GetValue(propertyInfo.Name, tmpCollectionType);

                    object propertyValue = null;
                    if (null != tmpCollection)
                    {
                        ConstructorInfo propertyConstructor =
                        propertyType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
                        propertyValue = propertyConstructor.Invoke(new object[0]);
                        foreach (object item in tmpCollection as IEnumerable)
                        {
                            addMethod.Invoke(propertyValue, new object[] { item });
                        }
                    }
                    propertyInfo.SetValue(obj, propertyValue);
                }
            }
        }
//
//        public static void FillCollectionDeserializationInfo(SerializationInfo info, object obj, Type type)
//        {
//            const string addMethodName = "Add";
//            Type[] arguments = type.GetGenericArguments();
//            if (0 == arguments.Length)
//            {
//                throw new InvalidOperationException();
//            }
//            Type elementType = arguments[0];
//            Type realElementType = arguments[0];
//            GenericCollectionAttribute genericAttribute;
//            if (null != (genericAttribute = elementType.GetCustomAttribute<GenericCollectionAttribute>()))
//            {
//                realElementType = genericAttribute.GenericType;
//            }
//            MethodInfo addMethod = type.GetMethod(addMethodName, BindingFlags.Instance | BindingFlags.Public, null,
//                CallingConventions.Standard, new Type[] { elementType }, new ParameterModifier[0]);
//
//            SerializationInfoEnumerator enumerator = info.GetEnumerator();
//            while (enumerator.MoveNext())
//            {
//                info.get
//            }
//        }

        #endregion

    }
}