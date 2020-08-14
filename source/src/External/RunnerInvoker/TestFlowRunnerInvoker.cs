using System;
using System.IO;
using System.Reflection;
using Testflow.Usr;

namespace Testflow.External.RunnerInvoker
{
    public static class TestFlowRunnerInvoker
    {
        public static TestflowRunner CreateInstance(TestflowRunnerOptions options)
        {
            string testflowHome = Environment.GetEnvironmentVariable("TESTFLOW_HOME");
            if (string.IsNullOrWhiteSpace(testflowHome))
            {
                throw new TestflowException(-1, "TestFlow platform cannot be found.");
            }
            if (!testflowHome.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                testflowHome += Path.DirectorySeparatorChar;
            }
            string activatorPath = $"{testflowHome}TestflowLauncher.dll";
            Assembly activatorAssembly = Assembly.LoadFrom(activatorPath);
            Type launcherType = activatorAssembly.GetType("Testflow.Loader.TestflowActivator", true);
            Type[] argumentTypes = new Type[] { typeof(TestflowRunnerOptions) };
            MethodInfo createMethod = launcherType.GetMethod("CreateRunner", BindingFlags.Public | BindingFlags.Static,
                null, argumentTypes, null);
            if (null == createMethod)
            {
                throw new TestflowException(-1, "TestFlow platform cannot be found.");
            }
            return (TestflowRunner) createMethod.Invoke(null, new object[] {options});
        }
    }
}
