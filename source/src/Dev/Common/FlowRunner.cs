using System;
using Testflow.ModuleInterface;

namespace Testflow
{
    public abstract class FlowRunner : IDisposable
    {
        private static FlowRunner _runnerInst;
        private static object _instLock;

        static FlowRunner()
        {
            _instLock = new object();
            _runnerInst = null;
        }

        /// <summary>
        /// 创建FlowRunner实例
        /// </summary>
        /// <param name="options">FlowRunner的选项配置</param>
        /// <returns></returns>
        public static FlowRunner GetInstance(FlowRunnerOptions options)
        {
            if (null != _runnerInst)
            {
                return _runnerInst;
            }
            lock(_instLock)
            {
                _runnerInst = GenerateFlowRunner(options);
            }
            return _runnerInst;
        }

        private static FlowRunner GenerateFlowRunner(FlowRunnerOptions options)
        {
            throw new System.NotImplementedException();
        }

        public abstract IComInterfaceLoader ComInterfaceLoader { get; }
        public abstract IConfigurationManager ConfigurationManager { get; }
        public abstract IDataPersistance DataPersistance { get; }
        public abstract IEngineController EngineController { get; }
        public abstract ILogService LogService { get; }
        public abstract IParameterChecker ParameterChecker { get; }
        public abstract IResultManager ResultManager { get; }
        public abstract ISequenceSerializer SequenceSerializer { get; }

        protected FlowRunnerOptions Option;

        public abstract void Initialize();

        public abstract void Dispose();
    }
}