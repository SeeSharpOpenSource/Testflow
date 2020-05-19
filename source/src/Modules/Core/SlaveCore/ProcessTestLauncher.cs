using System;
using System.Threading;
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

        private int _diposedFlag = 0;
        public void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            _slaveController.Dispose();
            // 日志会话最后释放结束
            _slaveContext.LogSession.Dispose();
        }
    }
}