using Testflow.Data.Sequence;

namespace Testflow.Runtime.OperationPanel
{
    /// <summary>
    /// OI配置窗体的接口
    /// </summary>
    public interface IOIConfigPanel
    {
        /// <summary>
        /// 初始化并显示OI配置窗体
        /// </summary>
        /// <param name="sequenceData">测试序列</param>
        /// <param name="extraParams">扩展参数，定义参见接口文档</param>
        void ShowOiConfigPanel(ISequenceFlowContainer sequenceData, params object[] extraParams);

        /// <summary>
        /// 返回配置窗体配置的参数
        /// </summary>
        /// <param name="isConfirmed">配置窗体是否点击了确认</param>
        /// <returns></returns>
        string GetParameter(out bool isConfirmed);
    }
}