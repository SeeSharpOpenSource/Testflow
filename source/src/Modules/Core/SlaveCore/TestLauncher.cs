using System.Collections.Generic;
using Newtonsoft.Json;
using Testflow.RemoteRunner.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore
{
    public class TestLauncher
    {
        private ContextManager _contextManager;
        private MessageTransceiver _transceiver;

        public TestLauncher(string configDataStr)
        {
            I18NOption i18NOption = new I18NOption(typeof(TestLauncher).Assembly, "i18n_SlaveCore_zh", "i18n_SlaveCore_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            _contextManager = new ContextManager(configDataStr);
            _transceiver = new MessageTransceiver(_contextManager);
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}