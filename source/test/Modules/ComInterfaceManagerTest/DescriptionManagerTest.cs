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
            // 未编写自动化监测，需要手动检查description的值
            IComInterfaceDescription description = _interfaceManager.GetComponentInterface(
                @"C:\SeeSharp\JYTEK\Hardware\DSA\JYPCI69527\Bin\JYPCI69527.dll");
            //            Assembly assembly =
            //                Assembly.LoadFile(@"C:\SeeSharp\JYTEK\SeeSharpTools\Bin\SeeSharpTools.JY.ArrayUtility.dll");
            //            IClassInterfaceDescription classDescription = description.Classes[0];
            //            Type classType =
            //                assembly.GetType($"{classDescription.ClassType.Namespace}.{classDescription.ClassType.Name}");
            //            IFuncInterfaceDescription funcDescription = classDescription.Functions[0];
            //            Type[] parameterTypes = new Type[funcDescription.Arguments.Count];
            //            ParameterModifier[] modifiers = new ParameterModifier[funcDescription.Arguments.Count];
            //            Type[] arguments = classType.GenericTypeArguments;
            //            Type[] genericArguments = classType.GetGenericArguments();
            //            for (int i = 0; i < funcDescription.Arguments.Count; i++)
            //            {
            //                string name = $"{funcDescription.Arguments[i].Type.Namespace}.{funcDescription.Arguments[i].Type.Name}";
            //                parameterTypes[i] = assembly.GetType(name);
            //                modifiers[i] = new ParameterModifier();
            //            }
            //            classType.GetMethod(funcDescription.Name, BindingFlags.Instance | BindingFlags.Public, null, parameterTypes, modifiers);
        }
    }
}