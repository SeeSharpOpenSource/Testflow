using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ComInterfaceManager
{
    internal class DescriptionLoaderManager : IDisposable
    {
        private AppDomain _loaderDomain;
//        private int _assemblyCount;
        private readonly string _assemblyFullName;
        private readonly string _loaderName;
        private AssemblyDescriptionLoader _loader;
        private readonly ISequenceManager _sequenceManager;

        public DescriptionLoaderManager()
        {
            _loaderDomain = AppDomain.CreateDomain(Constants.AppDomainName);
//            _loaderDomain.Load()
            Type loaderType = typeof(AssemblyDescriptionLoader);
//            _loaderName = $"{loaderType.Namespace}.{loaderType.Name}";
            _assemblyFullName = loaderType.Assembly.Location;
            _loaderName = loaderType.FullName;
            _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceFromAndUnwrap(_assemblyFullName, _loaderName);
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
        }

        public void LoadDefaultAssemblyDescription(DescriptionDataTable descriptionCollection, string testflowHome)
        {
            LoadMscorLibDescription(descriptionCollection);
            LoadFuncDefinitionDescriptions(descriptionCollection, testflowHome);
        }

        private void LoadMscorLibDescription(DescriptionDataTable descriptionCollection)
        {
            Assembly assembly = typeof (int).Assembly;
            ComInterfaceDescription mscoreLibDescription = _loader.LoadMscorlibDescription();

            IAssemblyInfo assemblyInfo = _sequenceManager.CreateAssemblyInfo();
            assemblyInfo.Path = assembly.Location;
            assemblyInfo.AssemblyName = assembly.GetName().Name;

            assemblyInfo.Version = assembly.GetName().Version.ToString();

            CheckAssemblyDescription(mscoreLibDescription, assemblyInfo.AssemblyName, assemblyInfo.Path);
            ModuleUtils.ValidateComDescription(_sequenceManager, mscoreLibDescription, descriptionCollection);
            assemblyInfo.Available = true;
            mscoreLibDescription.Assembly = assemblyInfo;
            descriptionCollection.Add(mscoreLibDescription);
        }

        private void LoadFuncDefinitionDescriptions(DescriptionDataTable descriptionCollection, string testflowHome)
        {
            string funcDefLibPath = $"{testflowHome}lib{Path.DirectorySeparatorChar}funcdefs.dll";
            LoadAssemblyDescription(funcDefLibPath, descriptionCollection);
        }

        public ComInterfaceDescription LoadAssemblyDescription(IAssemblyInfo assemblyInfo, 
            DescriptionDataTable descriptionCollection)
        {
            string assemblyName = assemblyInfo.AssemblyName;
            string version = assemblyInfo.Version;
            ComInterfaceDescription assemblyDescription = _loader.LoadAssemblyDescription(assemblyInfo.Path, ref assemblyName, ref version);
            CheckAssemblyDescription(assemblyDescription, assemblyInfo.AssemblyName, assemblyInfo.Path);

            assemblyInfo.Version = version;
            assemblyDescription.Assembly = assemblyInfo;

            ModuleUtils.ValidateComDescription(_sequenceManager, assemblyDescription, descriptionCollection);
            assemblyInfo.Available = true;
            descriptionCollection.Add(assemblyDescription);
            // 如果一个AppDomain载入过多的程序集，则卸载该AppDomain，构造新的AppDomain
//            if (_assemblyCount > Constants.MaxAssemblyCount)
//            {
//                AppDomain.Unload(_loaderDomain);
//                Thread.MemoryBarrier();
//                _loaderDomain = AppDomain.CreateDomain(Constants.AppDomainName);
//                _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceAndUnwrap(_assemblyFullName, _loaderName);
//                Interlocked.Exchange(ref _assemblyCount, 0);
//            }
            return assemblyDescription;
        }

        public ComInterfaceDescription LoadAssemblyDescription(string path, DescriptionDataTable descriptionCollection)
        {
            IAssemblyInfo assemblyInfo = _sequenceManager.CreateAssemblyInfo();
            assemblyInfo.Path = path;
            string assemblyName = string.Empty;
            string version = string.Empty;
            ComInterfaceDescription assemblyDescription = _loader.LoadAssemblyDescription(path, ref assemblyName, ref version);
            CheckAssemblyDescription(assemblyDescription, assemblyInfo.AssemblyName, path);

            assemblyInfo.AssemblyName = assemblyName;
            assemblyInfo.Version = version;
            assemblyDescription.Assembly = assemblyInfo;

            ModuleUtils.ValidateComDescription(_sequenceManager, assemblyDescription, descriptionCollection);
            assemblyInfo.Available = true;
            descriptionCollection.Add(assemblyDescription);
            // 如果一个AppDomain载入过多的程序集，则卸载该AppDomain，构造新的AppDomain
//            if (_assemblyCount > Constants.MaxAssemblyCount)
//            {
//                AppDomain.Unload(_loaderDomain);
//                Thread.MemoryBarrier();
//                _loaderDomain = AppDomain.CreateDomain(Constants.AppDomainName);
//                _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceAndUnwrap(_assemblyFullName, _loaderName);
//                Interlocked.Exchange(ref _assemblyCount, 0);
//            }
            return assemblyDescription;
        }

        public ITypeData GetPropertyType(ITypeData typeData, string property, DescriptionDataTable descriptionCollection)
        {
            ITypeDescription propertyTypeDescription = _loader.GetPropertyType( typeData.AssemblyName, 
                ModuleUtils.GetFullName(typeData),
                property);
            if (null == propertyTypeDescription)
            {
                CheckPropertyDescription(typeData, property);
            }
            string fullName = ModuleUtils.GetFullName(propertyTypeDescription);
            ITypeData propertyType;
            if (descriptionCollection.ContainsType(fullName))
            {
                propertyType = descriptionCollection.GetTypeData(fullName);
            }
            else
            {
                propertyType = _sequenceManager.CreateTypeData(propertyTypeDescription);
                descriptionCollection.AddTypeData(fullName, propertyType);
            }
            return propertyType;
        }

        public string[] GetEnumItemsByType(ITypeData typeData)
        {
            string typeFullName = ModuleUtils.GetFullName(typeData);
            string[] enumItems = _loader.GetEnumItems(typeData.AssemblyName, typeFullName);
            CheckEnumItems(enumItems, typeData);
            return enumItems;
        }

        public ClassInterfaceDescription GetClassDescription(ITypeData typeData, DescriptionDataTable descriptionDatas, ref string path, out string version)
        {
            string typeFullName = ModuleUtils.GetFullName(typeData);
            string assemblyName = typeData.AssemblyName;
            return GetClassDescription(descriptionDatas, assemblyName, typeFullName, ref path, out version);
        }

        public ClassInterfaceDescription GetClassDescription(DescriptionDataTable descriptionDatas, string assemblyName, string typeFullName, ref string path, out string version)
        {
            ClassInterfaceDescription classDescription = _loader.GetClassDescription(assemblyName,
                typeFullName, ref path, out version);
            // 初始化TypeData
            ITypeData classType = ModuleUtils.GetTypeDataByDescription(_sequenceManager, descriptionDatas,
                classDescription.ClassTypeDescription);
            classDescription.ClassType = classType;
            classDescription.ClassTypeDescription = null;
            foreach (IFuncInterfaceDescription funcDescription in classDescription.Functions)
            {
                funcDescription.ClassType = classType;
            }
            CheckClassDescription(classDescription, assemblyName, typeFullName);
            return classDescription;
        }

        public bool IsDerivedFrom(ITypeData typeData, ITypeData baseType)
        {
            string typeName = ModuleUtils.GetFullName(typeData);
            string baseTypeName = ModuleUtils.GetFullName(baseType);
            bool? result = _loader.IsDerivedFrom(typeData.AssemblyName, typeName, baseType.AssemblyName, baseTypeName);
            CheckDerivedResult(result, typeData, baseType);
            return result.Value;
        }

        #region AppDomain返回值校验

        private void CheckDerivedResult(bool? result, ITypeData typeData, ITypeData baseTypeData)
        {
            if (null != result)
            {
                return;
            }
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;
            switch (_loader.ErrorCode)
            {
                case ModuleErrorCode.LibraryLoadError:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assembly '{typeData.AssemblyName}/{baseTypeData.AssemblyName}' load error.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                        i18N.GetStr("RuntimeError"));
                    break;
                case ModuleErrorCode.TypeCannotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Type '{typeData.Name}/{baseTypeData.Name}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.TypeCannotLoad,
                        i18N.GetFStr("TypeNotFound", $"{typeData.Name}/{baseTypeData.Name}"));
                    break;
                case ModuleErrorCode.AssemblyNotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assemly '{typeData.AssemblyName}/{baseTypeData.AssemblyName}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.AssemblyNotLoad,
                        i18N.GetFStr("AssemblyNotLoad", $"{typeData.AssemblyName}/{baseTypeData.AssemblyName}"));
                    break;
                default:
                    break;
            }
        }

        private void CheckEnumItems(string[] enumItems, ITypeData typeData)
        {
            if (null != enumItems)
            {
                return;
            }
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;
            switch (_loader.ErrorCode)
            {
                case ModuleErrorCode.LibraryLoadError:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assembly '{typeData.AssemblyName}' load error.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                        i18N.GetStr("RuntimeError"));
                    break;
                case ModuleErrorCode.TypeCannotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Type '{typeData.Name}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.TypeCannotLoad,
                        i18N.GetFStr("TypeNotFound", typeData.Name));
                    break;
                case ModuleErrorCode.AssemblyNotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assemly '{typeData.AssemblyName}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.AssemblyNotLoad,
                        i18N.GetFStr("AssemblyNotLoad", typeData.AssemblyName));
                    break;
                default:
                    break;
            }
        }

        private void CheckPropertyDescription(ITypeData typeData, string property)
        {
            if (null != _loader.Exception)
            {
                return;
            }
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;
            switch (_loader.ErrorCode)
            {
                case ModuleErrorCode.LibraryLoadError:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assembly '{typeData.AssemblyName}' load error.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                        i18N.GetStr("RuntimeError"));
                    break;
                case ModuleErrorCode.PropertyNotFound:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Property '{property}' of type '{typeData.Name}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.PropertyNotFound,
                        i18N.GetFStr("PropertyNotFound", typeData.Name, property));
                    break;
                case ModuleErrorCode.TypeCannotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Type '{typeData.Name}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.TypeCannotLoad,
                        i18N.GetFStr("TypeNotFound", typeData.Name));
                    break;
                case ModuleErrorCode.AssemblyNotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assemly '{typeData.AssemblyName}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.AssemblyNotLoad,
                        i18N.GetFStr("AssemblyNotLoad", typeData.AssemblyName));
                    break;
                default:
                    break;
            }
        }

        private void CheckClassDescription(IClassInterfaceDescription description, string assemblyName, string fullName)
        {
            if (null != description && _loader.Exception == null && _loader.ErrorCode == 0)
            {
                return;
            }
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;
            string assembly = assemblyName;
            switch (_loader.ErrorCode)
            {
                case ModuleErrorCode.HighVersion:
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                        $"The version of assembly '{assembly}' is higher than version defined in data.");
                    break;
                case ModuleErrorCode.LowVersion:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"The version of assembly '{assembly}' is lower than version defined in data.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LowVersion,
                        i18N.GetFStr("LowAssemblyVersion", assembly));
                    break;
                case ModuleErrorCode.LibraryLoadError:
                    if (null != _loader.Exception)
                    {
                        logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                            $"Assembly '{assembly}' load error.");
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetFStr(_loader.Exception.Message), _loader.Exception);
                    }
                    else
                    {
                        logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                            $"Assembly '{assembly}' load error.");
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetStr("RuntimeError"));
                    }
                case ModuleErrorCode.TypeCannotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Type '{fullName}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.TypeCannotLoad,
                        i18N.GetFStr("TypeNotFound", fullName));
                case ModuleErrorCode.LibraryNotFound:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"Assembly '{assembly}' has not been loaded.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LibraryNotFound,
                        i18N.GetFStr("LibNotFound", assembly));
                case ModuleErrorCode.AssemblyNotLoad:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                        $"Assemly '{assemblyName}' does not exist.");
                    throw new TestflowRuntimeException(ModuleErrorCode.AssemblyNotLoad,
                        i18N.GetFStr("AssemblyNotLoad", assemblyName));
                    break;
                default:
                    if (null != _loader.Exception)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetFStr("RuntimeError", _loader.Exception.Message), _loader.Exception);
                    }
                    if (null == description)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetStr("RuntimeError"));
                    }
                    break;
            }
        }

        private void CheckAssemblyDescription(ComInterfaceDescription description, string assemblyName, string path)
        {
            if (null != description && _loader.Exception == null && _loader.ErrorCode == 0)
            {
                return;
            }
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;
            string assembly = assemblyName;
            if (string.IsNullOrWhiteSpace(assembly))
            {
                assembly = path;
            }
            switch (_loader.ErrorCode)
            {
                case ModuleErrorCode.HighVersion:
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                        $"The version of assembly '{assembly}' is higher than version defined in data.");
                    break;
                case ModuleErrorCode.LowVersion:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"The version of assembly '{assembly}' is lower than version defined in data.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LowVersion,
                        i18N.GetFStr("LowAssemblyVersion", assembly));
                    break;
                case ModuleErrorCode.LibraryLoadError:
                    if (null != _loader.Exception)
                    {
                        logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, _loader.Exception,
                            $"Assembly '{assembly}' load error.");
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetFStr(_loader.Exception.Message), _loader.Exception);
                    }
                    else
                    {
                        logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                            $"Assembly '{assembly}' load error.");
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetStr("RuntimeError"));
                    }
                case ModuleErrorCode.LibraryNotFound:
                    throw new TestflowRuntimeException(ModuleErrorCode.LibraryNotFound,
                        i18N.GetFStr("LibNotFound", assembly));
                default:
                    if (null != _loader.Exception)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetFStr("RuntimeError", _loader.Exception.Message), _loader.Exception);
                    }
                    if (null == description)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                            i18N.GetStr("RuntimeError"));
                    }
                    break;
            }
        }

        #endregion


        public void Dispose()
        {
            AppDomain.Unload(_loaderDomain);
        }

    }
}