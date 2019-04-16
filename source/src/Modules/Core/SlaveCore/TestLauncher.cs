using System.Collections.Generic;
using Newtonsoft.Json;
using Testflow.SlaveCore.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore
{
    public class TestLauncher
    {
        private SlaveContext _contextManager;
        private MessageTransceiver _transceiver;

        public TestLauncher(string configDataStr)
        {
            I18NOption i18NOption = new I18NOption(typeof(TestLauncher).Assembly, "i18n_SlaveCore_zh", "i18n_SlaveCore_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            _contextManager = new SlaveContext(configDataStr);
//            _transceiver = new MessageTransceiver(_contextManager, );
        }

        public void Start()
        {

        }
    }
}