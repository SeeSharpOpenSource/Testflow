using System;
using Testflow.Common;
using Testflow.DesignTime;
using Testflow.Modules;
using Testflow.Runtime;

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
        /// 获取或创建当前AppDomain下的FlowRunner实例
        /// </summary>
        /// <param name="options">FlowRunner的选项配置</param>
        /// <returns></returns>
        public static FlowRunner GetInstance(FlowRunnerOptions options)
        {
            CheckIfExistDifferentRunner(options);
            if (null != _runnerInst)
            {
                return _runnerInst;
            }
            lock(_instLock)
            {
                CheckIfExistDifferentRunner(options);
                if (null != _runnerInst)
                {
                    _runnerInst = GenerateFlowRunner(options);
                }
            }
            return _runnerInst;
        }

        private static void CheckIfExistDifferentRunner(FlowRunnerOptions options)
        {
            if (null != _runnerInst && !_runnerInst.Option.Equals(options))
            {
                throw new TestflowRuntimeException(-1, "A flowrunner instance with different option exist.");
            }
        }

        private static FlowRunner GenerateFlowRunner(FlowRunnerOptions options)
        {
            throw new System.NotImplementedException();
        }

        protected FlowRunner(FlowRunnerOptions options)
        {
            this.Option = options;
        }

        #region 服务接口

        /// <summary>
        /// 运行时服务
        /// </summary>
        public IRuntimeService RuntimeService { get; protected set; }

        /// <summary>
        /// 设计时服务
        /// </summary>
        public IDesignTimeService DesignTimeService { get; protected set; }

        #endregion

        #region 模块控制接口

        /// <summary>
        /// 组件接口加载模块
        /// </summary>
        public abstract IComInterfaceLoader ComInterfaceLoader { get; }

        /// <summary>
        /// 配置管理模块
        /// </summary>
        public abstract IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// 数据持久化模块
        /// </summary>
        public abstract IDataMaintainer DataPersistance { get; }

        /// <summary>
        /// 引擎控制模块
        /// </summary>
        public abstract IEngineController EngineController { get; }

        /// <summary>
        /// 日志服务模块
        /// </summary>
        public abstract ILogService LogService { get; }

        /// <summary>
        /// 参数检查模块
        /// </summary>
        public abstract IParameterChecker ParameterChecker { get; }

        /// <summary>
        /// 结果管理模块
        /// </summary>
        public abstract IResultManager ResultManager { get; }

        /// <summary>
        /// 序列化模块
        /// </summary>
        public abstract ISequenceSerializer SequenceSerializer { get; }

        #endregion

        #region 辅助组件接口

        /// <summary>
        /// 国际化组件
        /// </summary>
        public abstract I18NInterface I18n { get; }

        #endregion


        /// <summary>
        /// 运行器选项
        /// </summary>
        public FlowRunnerOptions Option;

        /// <summary>
        /// 初始化框架平台
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 销毁当前Runner
        /// </summary>
        public abstract void Dispose();
    }
}