using System;

namespace Testflow
{
    /// <summary>
    /// Testflow的控制台接口
    /// </summary>
    public interface IFlowConsole
    {
        /// <summary>
        /// 在控制台打印信息
        /// </summary>
        /// <param name="message">待打印的消息</param>
        void Print(string message);

        /// <summary>
        /// 在控制台打印信息
        /// </summary>
        /// <param name="messageFormat">消息格式</param>
        /// <param name="args">消息的参数</param>
        void Print(string messageFormat, params object[] args);

        /// <summary>
        /// 在控制台打印信息
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="message">附加信息</param>
        void Print(Exception exception, string message = null);
    }
}