using System;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 测试生成报告消息
    /// </summary>
    [Serializable]
    public class TestGenMessage : MessageBase
    {
        // TODO 暂时停止到Sequence级别的报告
//        public int SequenecIndex { get; set; }

        public GenerationStatus State { get; set; }

        public string ErrorInfo { get; set; }

        public CallStack ErrorStack { get; set; }

        public TestGenMessage(string name, int id, int sequenceIndex, GenerationStatus state) : base(name, id, MessageType.TestGen)
        {
//            this.SequenecIndex = sequenceIndex;
            this.State = state;
            this.ErrorInfo = null;
            this.ErrorStack = null;
        }

        public TestGenMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
//            this.SequenecIndex = (int) info.GetValue("SequenceIndex", typeof(int));
            this.State = (GenerationStatus) info.GetValue("State", typeof(GenerationStatus));
            this.ErrorInfo = (string) info.GetString("State");
            this.ErrorStack = (CallStack) info.GetValue("ErrorStack", typeof(CallStack));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("State", State);
            info.AddValue("ErrorInfo", ErrorInfo, typeof(string));
            info.AddValue("ErrorStack", ErrorStack, typeof(CallStack));
        }
    }
}