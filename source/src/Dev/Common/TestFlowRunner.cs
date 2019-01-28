using System;
using Testflow.Common;
using Testflow.DesignTime;
using Testflow.I18nUtil;
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
        /// <param name="options">FlowRunner的选项配置</param>
        /// <returns></returns>
        public static TestflowRunner GetInstance(TestflowRunnerOptions options)
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
                    throw new TestflowInternalException(TestflowErrorCode.InternalError, i18N.GetStr("PlatformNotInitialized"));
                }
            }
            return _runnerInst;
        }

        private static void CheckIfExistDifferentRunner(TestflowRunnerOptions options)
        {
            if (null != _runnerInst && !_runnerInst.Option.Equals(options))
            {
                throw new TestflowRuntimeException(-1, "A flowrunner instance with different option exist.");
            }
        }

        private static TestflowRunner GenerateFlowRunner(TestflowRunnerOptions options)
        {
            throw new System.NotImplementedException();
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
                "Testflow.Resources.locale.i18n_common_zh", "Testflow.Resources.locale.i18n_common_en")
            {
                Name = CommonConst.I18nName
            };
            I18N i18N = I18N.GetInstance(i18NOption);
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
        public abstract IComInterfaceManager ComInterfaceManager { get; }

        /// <summary>
        /// 配置管理模块
        /// </summary>
        public abstract IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// 数据持久化模块
        /// </summary>
        public abstract IDataMaintainer DataMaintainer { get; }

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
        /// 序列管理模块
        /// </summary>
        public abstract ISequenceManager SequenceManager { get; }

        #endregion

        /// <summary>
        /// 运行器选项
        /// </summary>
        public TestflowRunnerOptions Option { get; }

        /// <summary>
        /// Testflow平台的上下文信息
        /// </summary>
        public TestflowContext Context { get; }

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