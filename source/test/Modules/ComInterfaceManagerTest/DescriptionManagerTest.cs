using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.ComInterfaceManager;
using Testflow.Data.Description;
using Testflow.Modules;

namespace Testflow.ComInterfaceManagerTest
{
    [TestClass]
    public class DescriptionManagerTest
    {
        private IComInterfaceManager _interfaceManager;

        public DescriptionManagerTest()
        {
            Type runnerType = typeof(TestflowRunner);
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, fakeTestflowRunner);
            fakeTestflowRunner.Initialize();

            _interfaceManager = fakeTestflowRunner.ComInterfaceManager;
            _interfaceManager.DesigntimeInitialize();
        }

        [TestMethod]
        public void LoadDllInterfaceTest()
        {
            IComInterfaceDescription description = _interfaceManager.GetComponentInterface(
                @"C:\SeeSharp\JYTEK\SeeSharpTools\Bin\SeeSharpTools.JY.ArrayUtility.dll");
        }
    }
}