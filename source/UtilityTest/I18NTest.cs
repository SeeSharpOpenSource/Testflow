using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Dev.UtilityTest.Resources;
using Testflow.Utility.I18nUtil;

namespace Testflow.Dev.UtilityTest
{
    [TestClass]
    public class I18NTest
    {
        private I18N _i18N;
        private I18NOption _i18NOption;
        const string I18nName = "i18nTest";
        const string TestLabel = "TestLabel";
        [TestInitialize]
        public void SetUp()
        {
            _i18NOption = new I18NOption(typeof (I18NTest).Assembly, "i18n_test_cn",
                "i18n_test_en")
            {
                Name = I18nName
            };
            _i18N = I18N.GetInstance(_i18NOption);
        }

        [TestMethod]
        public void GetEntity()
        {
            I18N i18N = I18N.GetInstance(I18nName);
            Assert.AreNotEqual(null, i18N);
        }

        [TestMethod]
        public void I18NChineseTest()
        {
            I18N.RemoveInstance(I18nName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN");
            _i18N = I18N.GetInstance(_i18NOption);
            string str = _i18N.GetStr(TestLabel);
            Assert.AreEqual(str, i18n_test_cn.TestLabel);

        }

        [TestMethod]
        public void I18NEnglishTest()
        {
            I18N.RemoveInstance(I18nName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            _i18N = I18N.GetInstance(_i18NOption);
            string str = _i18N.GetStr(TestLabel);
            Assert.AreEqual(str, i18n_test_en.TestLabel);
        }
    }
}
