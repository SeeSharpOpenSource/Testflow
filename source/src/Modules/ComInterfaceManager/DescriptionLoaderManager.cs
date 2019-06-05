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
        private int _assemblyCount;
        private readonly string _assemblyFullName;
        private readonly string _loaderName;
        private AssemblyDescriptionLoader _loader;
        private readonly ISequenceManager _sequenceManager;

        public DescriptionLoaderManager()
        {
            _loaderDomain = AppDomain.CreateDomain(Constants.AppDomainName);
            _assemblyFullName = this.GetType().Assembly.FullName;
            _loaderName = typeof(AssemblyDescriptionLoader).Name;
            _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceAndUnwrap(_assemblyFullName, _loaderName);
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
        }

        public ComInterfaceDescription LoadAssemblyDescription(IAssemblyInfo assemblyInfo, 
            DescriptionDataTable descriptionCollection)
        {
            ComInterfaceDescription assemblyDescription = _loader.LoadAssemblyDescription(assemblyInfo);
            CheckAssemblyDescription(assemblyDescription);
            ModuleUtils.ValidateComDescription(_sequenceManager, assemblyDescription, descriptionCollection);
            descriptionCollection.Add(assemblyDescription);
            // 如果一个AppDomain载入过多的程序集，则卸载该AppDomain，构造新的AppDomain
            if (_assemblyCount > Constants.MaxAssemblyCount)
            {
                AppDomain.Unload(_loaderDomain);
                Thread.MemoryBarrier();
                _loaderDomain = AppDomain.CreateDomain(Constants.AppDomainName);
                _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceAndUnwrap(_assemblyFullName, _loaderName);
                Interlocked.Exchange(ref _assemblyCount, 0);
            }
            return assemblyDescription;
        }

        public ComInterfaceDescription LoadAssemblyDescription(string path, DescriptionDataTable descriptionCollection)
        {
            IAssemblyInfo assemblyInfo = _sequenceManager.CreateAssemblyInfo();
            assemblyInfo.Path = path;
            ComInterfaceDescription assemblyDescription = _loader.LoadAssemblyDescription(assemblyInfo);
            CheckAssemblyDescription(assemblyDescription);
            ModuleUtils.ValidateComDescription(_sequenceManager, assemblyDescription, descriptionCollection);
            descriptionCollection.Add(assemblyDescription);
            // 如果一个AppDomain载入过多的程序集，则卸载该AppDomain，构造新的AppDomain
            if (_assemblyCount > Constants.MaxAssemblyCount)
            {
                AppDomain.Unload(_loaderDomain);
                Thread.MemoryBarrier();
                _loaderDomain = AppDomain.CreateDomain(Constants.AppDomainName);
                _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceAndUnwrap(_assemblyFullName, _loaderName);
                Interlocked.Exchange(ref _assemblyCount, 0);
            }
            return assemblyDescription;
        }

        private void CheckAssemblyDescription(ComInterfaceDescription description)
        {
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;
            if (null == description && null != _loader.Exception)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError,
                    i18N.GetFStr("RuntimeError", _loader.Exception.Message), _loader.Exception);
            }
            switch (_loader.ErrorCode)
            {
                case ModuleErrorCode.HighVersion:

                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                        $"The version of assembly '{description.Assembly.AssemblyName}' is higher than version defined in data.");
                    break;
                case ModuleErrorCode.LowVersion:
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"The version of assembly '{description.Assembly.AssemblyName}' is lower than version defined in data.");
                    throw new TestflowRuntimeException(ModuleErrorCode.LowVersion,
                        i18N.GetFStr("LowAssemblyVersion", description.Assembly.AssemblyName));
                    break;
                case ModuleErrorCode.LibraryLoadError:
                    throw new TestflowRuntimeException(ModuleErrorCode.LibraryLoadError, i18N.GetStr("RuntimeError"));
                default:
                    break;
            }
        }

        public void Dispose()
        {
            AppDomain.Unload(_loaderDomain);
        }
    }
}