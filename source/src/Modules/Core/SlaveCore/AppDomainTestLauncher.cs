using System;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Controller;
using Testflow.SlaveCore.Runner;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore
{
    public class AppDomainTestLauncher : MarshalByRefObject, IDisposable
    {
        private SlaveContext _slaveContext;

        public AppDomainTestLauncher(string configDataStr)
        {
            I18NOption i18NOption = new I18NOption(typeof(TestLauncher).Assembly, "i18n_SlaveCore_zh", "i18n_SlaveCore_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            _slaveContext = new SlaveContext(configDataStr);
        }

        public void Start()
        {
            SlaveController slaveController = new SlaveController(_slaveContext);
            _slaveContext.Controller = slaveController;
            slaveController.StartslaveTask();
        }

        public void Dispose()
        {
            _slaveContext.Dispose();
        }
    }
}