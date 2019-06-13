using System;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Utility.MessageUtil;

namespace Testflow.Dev.UtilityTest
{
    [TestClass]
    public class SyncMessengeTest
    {
        private Messenger _messenger;
        private MessengerOption _option;
        const string MsgQueueName = @".\Private$\TestflowTest";
        const int MaxSession = 50;

        [TestInitialize]
        public void SetUp()
        {
            _option = new MessengerOption(MsgQueueName, GetMessageType)
            {
                ReceiveType = ReceiveType.Synchronous
            };
            _messenger = Messenger.GetMessenger(_option);
        }

        private Type GetMessageType(string label)
        {
            return typeof (TestMessage);
        }

        [TestMethod]
        public void GetMessage()
        {
            Messenger messenger = Messenger.GetMessenger(_option);
            Assert.IsNotNull(messenger);
        }

        [TestMethod]
        public void TestSyncReceive()
        {
            TestMessage message = new TestMessage()
            {
                Id = -5,
                Message = CreateTestMessage(-5)
            };
            _messenger.Send(message);

            TestMessage peakMessage = _messenger.Peak() as TestMessage;
            Assert.AreEqual(message.Id, peakMessage.Id);
            Assert.AreEqual(message.Message, peakMessage.Message);
            Assert.AreEqual(_messenger.MessageCount, 1);

            TestMessage receiveMessage = _messenger.Receive() as TestMessage;
            Assert.AreEqual(message.Id, receiveMessage.Id);
            Assert.AreEqual(message.Message, receiveMessage.Message);
            Assert.AreEqual(_messenger.MessageCount, 0);
        }

        [TestCleanup]
        public void TearDown()
        {
            Messenger.DestroyMessenger(MsgQueueName);
        }

        public static string CreateTestMessage(int id)
        {
            return $"Message to session {id}.";
        }
    }
}
