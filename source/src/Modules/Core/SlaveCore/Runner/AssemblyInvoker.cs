using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner
{
    internal class AssemblyInvoker
    {
        private readonly Dictionary<string, Assembly> _assembliesMapping;
        private readonly Dictionary<string, Type> _typeDataMapping;

        private readonly Dictionary<string, Func<string, object>> _valueTypeConvertors;

        private readonly IAssemblyInfoCollection _assemblyInfos;
        private readonly ITypeDataCollection _typeDatas;

        private readonly SlaveContext _context;

        private readonly string _dotNetLibDir;
        private readonly string _platformLibDir;
        private readonly string[] _instanceLibDir;

        public AssemblyInvoker(SlaveContext context, IAssemblyInfoCollection assemblyInfo, ITypeDataCollection typeDatas)
        {
            this._assembliesMapping = new Dictionary<string, Assembly>(assemblyInfo.Count);
            this._typeDataMapping = new Dictionary<string, Type>(typeDatas.Count);

            this._assemblyInfos = assemblyInfo;
            this._typeDatas = typeDatas;

            this._context = context;

            this._dotNetLibDir = context.GetProperty<string>("DotNetLibDir");
            this._platformLibDir = context.GetProperty<string>("PlatformLibDir");
            this._instanceLibDir = context.GetProperty<string>("InstanceLibDir").Split(';');

            _valueTypeConvertors = new Dictionary<string, Func<string, object>>(20)
            {
                {typeof (double).Name, valueStr => double.Parse(valueStr)},
                {typeof (float).Name, valueStr => float.Parse(valueStr)},
                {typeof (long).Name, valueStr => long.Parse(valueStr)},
                {typeof (ulong).Name, valueStr => ulong.Parse(valueStr)},
                {typeof (int).Name, valueStr => int.Parse(valueStr)},
                {typeof (uint).Name, valueStr => uint.Parse(valueStr)},
                {typeof (short).Name, valueStr => short.Parse(valueStr)},
                {typeof (ushort).Name, valueStr => ushort.Parse(valueStr)},
                {typeof (char).Name, valueStr => char.Parse(valueStr)},
                {typeof (byte).Name, valueStr => byte.Parse(valueStr)},
                {typeof (bool).Name, valueStr => bool.Parse(valueStr)},
                {typeof (decimal).Name, valueStr => decimal.Parse(valueStr)},
                {typeof (sbyte).Name, valueStr => sbyte.Parse(valueStr)},
                {typeof (DateTime).Name, valueStr => DateTime.Parse(valueStr)},
            };


        }

        public void LoadAssemblyAndType()
        {
            LoadAssemblies();
            LoadTypes();
        }

        public Type GetType(ITypeData typeData)
        {
            return _typeDataMapping[ModuleUtils.GetTypeFullName(typeData)];
        }

        public MethodInfo GetMethod(IFunctionData function)
        {
            BindingFlags bindingFlags = BindingFlags.Public;
            MethodInfo methodInfo;
            Type[] parameterTypes;
            ParameterModifier[] modifiers = null;
            Type classType = _typeDataMapping[ModuleUtils.GetTypeFullName(function.ClassType)];
            switch (function.Type)
            {
                case FunctionType.InstanceFunction:
                    bindingFlags |= BindingFlags.Instance;
                    break;
                case FunctionType.StaticFunction:
                    bindingFlags |= BindingFlags.Static;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            parameterTypes = new Type[function.ParameterType.Count];
            if (function.ParameterType.Count > 0)
            {
                modifiers = new ParameterModifier[] { new ParameterModifier(function.ParameterType.Count) };
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    parameterTypes[i] = _typeDataMapping[ModuleUtils.GetTypeFullName(function.ParameterType[i].Type)];
                    if (function.ParameterType[i].Modifier != ArgumentModifier.None && !parameterTypes[i].IsByRef)
                    {
                        modifiers[0][i] = true;
                        parameterTypes[i] = parameterTypes[i].MakeByRefType();
                    }
                }
            }
            
            methodInfo = classType.GetMethod(function.MethodName, bindingFlags, null, parameterTypes, modifiers);
            return methodInfo;
        }

        public ConstructorInfo GetConstructor(IFunctionData function)
        {
            BindingFlags bindingFlags = BindingFlags.Public;
            ConstructorInfo constructor;
            Type[] parameterTypes;
            ParameterModifier[] modifiers = null;
            Type classType = _typeDataMapping[ModuleUtils.GetTypeFullName(function.ClassType)];
            switch (function.Type)
            {
                case FunctionType.Constructor:
                    bindingFlags |= BindingFlags.Instance;
                    parameterTypes = new Type[function.ParameterType.Count];
                    if (function.ParameterType.Count > 0)
                    {
                        modifiers = new ParameterModifier[] { new ParameterModifier(function.ParameterType.Count) };
                        for (int i = 0; i < parameterTypes.Length; i++)
                        {
                            parameterTypes[i] = _typeDataMapping[ModuleUtils.GetTypeFullName(function.ParameterType[i].Type)];
                            if (function.ParameterType[i].Modifier != ArgumentModifier.None && !parameterTypes[i].IsByRef)
                            {
                                modifiers[0][i] = true;
                                parameterTypes[i] = parameterTypes[i].MakeByRefType();
                            }
                        }
                    }
                    constructor = classType.GetConstructor(bindingFlags, null, parameterTypes, modifiers);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return constructor;
        }

        public object CastConstantValue(ITypeData type, string valueStr)
        {
            Type dataType = _typeDataMapping[ModuleUtils.GetTypeFullName(type)];
            return _context.Convertor.CastConstantValue(dataType, valueStr);
        }

        private void LoadAssemblies()
        {
            try
            {
                foreach (IAssemblyInfo assemblyInfo in _assemblyInfos)
                {
                    string fullPath = GetAssemblyFullPath(assemblyInfo.Path);
                    if (null == fullPath)
                    {
                        _context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, 
                            $"Assembly '{assemblyInfo.AssemblyName}' cannot be found in path '{assemblyInfo.Path}'.");
                        throw new TestflowRuntimeException(ModuleErrorCode.UnavailableLibrary, 
                            _context.I18N.GetFStr("UnexistLibrary", assemblyInfo.AssemblyName, assemblyInfo.Path));
                    }
                    Assembly assembly = Assembly.LoadFrom(fullPath);
                    CheckVersion(assemblyInfo.Version, assembly);
                    _assembliesMapping.Add(assemblyInfo.AssemblyName, assembly);
                }
            }
            catch (TestflowException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Load assembly failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.UnavailableLibrary, _context.I18N.GetStr("LoadAssemblyFailed"), ex);
            }
        }

        private void LoadTypes()
        {
            string typeName = string.Empty;
            string assemblyName = string.Empty;
            try
            {
                foreach (ITypeData typeData in _typeDatas)
                {
                    Assembly assembly = _assembliesMapping[typeData.AssemblyName];
                    string fullName = ModuleUtils.GetTypeFullName(typeData);

                    typeName = fullName;
                    assemblyName = typeData.AssemblyName;

                    Type type = LoadType(assembly, typeData);
                    _typeDataMapping.Add(fullName, type);
                }
            }
            catch (TypeLoadException ex)
            {
                _context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, 
                    $"Type '{typeName}' cannot found in assembly '{assemblyName}'.");
                throw new TestflowDataException(ModuleErrorCode.UnaccessibleType,
                    _context.I18N.GetStr("LoadTypeFailed"), ex);
            }
            catch (Exception ex)
            {
                _context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Load type failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.UnaccessibleType,
                    _context.I18N.GetStr("LoadTypeFailed"), ex);
            }
        }

        private static Type LoadType(Assembly assembly, ITypeData typeData)
        {
            const string delim = ".";
            // 不包含域操作符则说明该类型不是nestedType，直接通过程序集获取类型
            if (!typeData.Name.Contains(delim))
            {
                return assembly.GetType(ModuleUtils.GetTypeFullName(typeData), true, false);
            }
            string[] nestedNames = typeData.Name.Split(delim.ToCharArray());
            string topLevelTypeName = ModuleUtils.GetTypeFullName(typeData.Namespace, nestedNames[0]);
            // 获取最上层的Type类型，再依次向下
            Type realType = assembly.GetType(topLevelTypeName, true, false);
            for (int i = 1; i < nestedNames.Length; i++)
            {
                realType = realType.GetNestedType(nestedNames[i]);
            }
            return realType;
        }

        private string GetAssemblyFullPath(string path)
        {
            if (ModuleUtils.IsAbosolutePath(path))
            {
                return File.Exists(path) ? path : null;
            }
            string fullPath = null;
            foreach (string libDir in _instanceLibDir)
            {
                fullPath = ModuleUtils.GetFileFullPath(path, libDir);
                if (null != fullPath)
                {
                    return fullPath;
                }
            }
            fullPath = ModuleUtils.GetFileFullPath(path, _platformLibDir);
            if (null != fullPath)
            {
                return fullPath;
            }
            fullPath = ModuleUtils.GetFileFullPath(path, _dotNetLibDir);
            return fullPath;
        }

        private void CheckVersion(string writeVersion, Assembly assembly)
        {
            const char delim = '.';
            string[] versionElem = writeVersion.Split(delim);
            int major = int.Parse(versionElem[0]);
            int minor = int.Parse(versionElem[1]);
            int build = versionElem.Length >= 3 ? int.Parse(versionElem[2]) : 0;
            int revision = versionElem.Length >= 4 ? int.Parse(versionElem[3]) : 0;

            Version libVersion = assembly.GetName().Version;

            if (libVersion.Major == major && libVersion.Minor == minor && libVersion.Build == build &&
                libVersion.Revision == revision)
            {
                return;
            }
            long libVersionNum = libVersion.Major * (long)10E9 + libVersion.Minor * (long)10E6 + libVersion.Build * (long)10E3 +
                                 libVersion.Revision;
            long versionNum = major * (long)10E9 + minor * (long)10E6 + build * (long)10E3 + revision;
            if (libVersionNum > versionNum)
            {
                string assmblyName = assembly.GetName().Name;
                _context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 
                    $"The version of library {assmblyName} is higher than the version defined in sequence.");
            }
            else
            {
                string assmblyName = assembly.GetName().Name;
                _context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, $"The version of library {assmblyName} is lower than the version defined in sequence.");
                throw new TestflowDataException(ModuleErrorCode.UnavailableLibrary, _context.I18N.GetFStr("InvalidLibVersion", assmblyName));
            }
        }
    }
}