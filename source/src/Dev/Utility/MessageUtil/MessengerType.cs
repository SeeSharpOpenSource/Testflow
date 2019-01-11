namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 信使类型
    /// </summary>
    public enum MessengerType
    {
        /// <summary>
        /// 使用MSMQ作为消息队列
        /// </summary>
        MSMQ = 0,

        /// <summary>
        /// 使用卡夫卡作为消息队列
        /// </summary>
        Kafka = 1,
    }
}