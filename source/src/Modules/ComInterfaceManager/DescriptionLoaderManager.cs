using System;
using System.Reflection;
using System.Threading;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;

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
            _loaderName = typeof(AssemblyDescriptionLoader).FullName;
            _loader = (AssemblyDescriptionLoader)_loaderDomain.CreateInstanceAndUnwrap(_assemblyFullName, _loaderName);
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
        }


        public ComInterfaceDescription LoadAssemblyDescription(IAssemblyInfo assemblyInfo, 
            DescriptionCollections descriptionCollection)
        {
            ComInterfaceDescription assemblyDescription = _loader.LoadAssemblyDescription(assemblyInfo);
            ModuleUtils.ValidateComDescription(_sequenceManager, assemblyDescription, descriptionCollection);
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

        public void Dispose()
        {
            AppDomain.Unload(_loaderDomain);
        }
    }
}