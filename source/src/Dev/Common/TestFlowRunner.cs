using System;
using System.Threading;
using Testflow.Usr;
using Testflow.DesignTime;
using Testflow.Usr.I18nUtil;
using Testflow.Modules;
using Testflow.Runtime;

namespace Testflow
{
    /// <summary>
    /// Testflow框架平台的运行器
    /// </summary>
    public abstract class TestflowRunner : IDisposable
    {
        private static TestflowRunner _runnerInst;
        private static object _instLock;

        static TestflowRunner()
        {
            _instLock = new object();
            _runnerInst = null;
        }

        /// <summary>
        /// 获取或创建当前AppDomain下的FlowRunner实例
        /// </summary>
        /// <returns></returns>
        protected static void SetInstance(TestflowRunner runnerInstance)
        {
            lock (_instLock)
            {
                if (null != _runnerInst && ReferenceEquals(_runnerInst, runnerInstance))
                {
                    throw new TestflowRuntimeException(-1, "A flowrunner instance with different option exist.");
                }
                _runnerInst = runnerInstance;
            }
        }

        /// <summary>
        /// 获取或创建当前AppDomain下的FlowRunner实例
        /// </summary>
        /// <returns></returns>
        public static TestflowRunner GetInstance()
        {
            if (null != _runnerInst)
            {
                return _runnerInst;
            }
            lock (_instLock)
            {
                if (null != _runnerInst)
                {
                    I18N i18N = I18N.GetInstance(CommonConst.I18nName);
                    throw new TestflowInternalException(CommonErrorCode.InternalError, 
                        i18N.GetStr("PlatformNotInitialized"));
                }
            }
            return _runnerInst;
        }

        /// <summary>
        /// 抽象类的构造方法
        /// </summary>
        /// <param name="options"></param>
        protected TestflowRunner(TestflowRunnerOptions options)
        {
            this.Option = options;
            this.Context = new TestflowContext();
            I18NOption i18NOption = new I18NOption(typeof (TestflowRunner).Assembly,
                "i18n_common_cn", "i18n_common_en")
            {
                Name = CommonConst.I18nName
            };
            I18N.GetInstance(i18NOption);
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
        public IComInterfaceManager ComInterfaceManager { get; protected set; }

        /// <summary>
        /// 配置管理模块
        /// </summary>
        public IConfigurationManager ConfigurationManager { get; protected set; }

        /// <summary>
        /// 数据持久化模块
        /// </summary>
        public IDataMaintainer DataMaintainer { get; protected set; }

        /// <summary>
        /// 引擎控制模块
        /// </summary>
        public IEngineController EngineController { get; protected set; }

        /// <summary>
        /// 日志服务模块
        /// </summary>
        public ILogService LogService { get; protected set; }

        /// <summary>
        /// 参数检查模块
        /// </summary>
        public IParameterChecker ParameterChecker { get; protected set; }

        /// <summary>
        /// 结果管理模块
        /// </summary>
        public IResultManager ResultManager { get; protected set; }

        /// <summary>
        /// 序列管理模块
        /// </summary>
        public ISequenceManager SequenceManager { get; protected set; }

        #endregion

        /// <summary>
        /// 运行器选项
        /// </summary>
        public TestflowRunnerOptions Option { get; protected set; }

        /// <summary>
        /// Testflow平台的上下文信息
        /// </summary>
        public TestflowContext Context { get; protected set; }

        /// <summary>
        /// 初始化框架平台
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 设计时初始化
        /// </summary>
        public virtual void DesigntimeInitialize()
        {
            RuntimeService.Deactivate();
            ConfigurationManager.DesigntimeInitialize();
            LogService.DesigntimeInitialize();
            ParameterChecker.DesigntimeInitialize();
            ResultManager.DesigntimeInitialize();
            ComInterfaceManager.DesigntimeInitialize();
            SequenceManager.DesigntimeInitialize();
            DataMaintainer.DesigntimeInitialize();
            EngineController.DesigntimeInitialize();
            DesignTimeService.Activate();
        }

        /// <summary>
        /// 运行时初始化
        /// </summary>
        public virtual void RuntimeInitialize()
        {
            DesignTimeService?.Deactivate();
            ConfigurationManager.RuntimeInitialize();
            LogService.RuntimeInitialize();
            ResultManager.RuntimeInitialize();
            SequenceManager.RuntimeInitialize();
            DataMaintainer.RuntimeInitialize();
            EngineController.RuntimeInitialize();
            RuntimeService.Activate();
        }

        /// <summary>
        /// 销毁当前Runner
        /// </summary>
        public virtual void Dispose()
        {
            _runnerInst = null;
            DesignTimeService?.Dispose();
            RuntimeService?.Dispose();
            EngineController?.Dispose();
            DataMaintainer?.Dispose();
            SequenceManager?.Dispose();
            ComInterfaceManager?.Dispose();
            ResultManager?.Dispose();
            ParameterChecker?.Dispose();
            LogService?.Dispose();
            ConfigurationManager?.Dispose();
        }
    }
}