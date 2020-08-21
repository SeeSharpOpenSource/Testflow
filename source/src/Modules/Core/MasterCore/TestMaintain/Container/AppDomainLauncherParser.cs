using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Testflow.CoreCommon;
using Testflow.MasterCore.Common;
using Testflow.SlaveCore;
using Testflow.Usr;

namespace Testflow.MasterCore.TestMaintain.Container
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    internal class AppDomainLauncherParser
    {
        private AppDomain _localDomain;
        private ModuleGlobalInfo _globalInfo;

        /// <summary>
        /// Creates a new instance of the WsdlParser in a new AppDomain
        /// </summary>
        /// <returns></returns>        
        public AppDomainTestLauncher CreateAppDomainLauncher(ModuleGlobalInfo globalInfo, int sessionId, string configStr)
        {
            this._globalInfo = globalInfo;
            this.CreateAppDomain(string.Format(Constants.SlaveRunnerNameFormat, sessionId));
            string testflowHome = _globalInfo.ConfigData.GetProperty<string>("TestflowHome");
            string corePath = $"{testflowHome}SlaveCore.dll";
            AppDomainTestLauncher launcher = (AppDomainTestLauncher) _localDomain.CreateInstanceFromAndUnwrap(
                corePath, Constants.AppDomainLauncherName, false, BindingFlags.Instance | BindingFlags.Public,
                null, new object[] { configStr }, CultureInfo.CurrentCulture, null);
            return launcher;
        }

        public void LauncherOver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        private void CreateAppDomain(string appDomain)
        {
            AppDomainSetup domainSetup = new AppDomainSetup
            {
                ApplicationName = appDomain,
//                ApplicationBase = _globalInfo.ConfigData.GetProperty<string>("TestflowHome")
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };

            // AppDomain.CurrentDomain.BaseDirectory;                 
            _localDomain = AppDomain.CreateDomain(appDomain, null, domainSetup);

            // *** Need a custom resolver so we can load assembly from non current path
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                return Assembly.Load(args.Name);
            }
            catch (Exception ex)
            {
                string[] parts = args.Name.Split(',');
                string file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + parts[0].Trim() + ".dll";
                return Assembly.LoadFrom(file);
//                throw new TestflowRuntimeException(ModuleErrorCode.RuntimeError, ex.Message, ex);
            }
        }

        /// <summary>
        /// 卸载容器
        /// </summary>
        public void Unload()
        {
            if (null != _localDomain && !_localDomain.IsFinalizingForUnload())
            {
                AppDomain.Unload(_localDomain);
                _localDomain = null;
            }
        }
    }
}