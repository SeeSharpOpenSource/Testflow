using System;
using Testflow.SlaveCore.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore
{
    public class ProcessTestLauncher : IDisposable
    {
        private SlaveContext _slaveContext;
        private SlaveController _slaveController;

        public ProcessTestLauncher(string configDataStr)
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
            _slaveController = new SlaveController(_slaveContext);
            _slaveController.StartSlaveTask();
        }

        public void Dispose()
        {
            _slaveController.Dispose();
        }
    }
}