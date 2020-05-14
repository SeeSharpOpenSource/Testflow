using Testflow.SlaveCore;

namespace Testflow.SlaveRunnerX86
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessTestLauncher testLauncher = null;
            try
            {
                testLauncher = new ProcessTestLauncher(args[0]);
                testLauncher.Start();
            }
            finally
            {
                testLauncher?.Dispose();
            }
        }
    }
}
