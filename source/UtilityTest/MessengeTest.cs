using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Utility.MessageUtil;

namespace Testflow.Dev.UtilityTest
{
    [TestClass]
    public class MessengeTest
    {
        private Messenger _messenger;
        private MessengerOption _option;
        const string MsgQueueName = "Test";
        const int MaxSession = 10;

        [TestInitialize]
        public void SetUp()
        {
            _option = new MessengerOption(MsgQueueName, new Type[] {typeof (TestMessage)});
            _messenger = Messenger.CreateMessenger(_option);
            IMessageConsumer[] consumers = new IMessageConsumer[MaxSession];
            for (int i = 0; i < MaxSession; i++)
            {
                consumers[i] = new TestConsumer(i);
            }
            _messenger.Initialize(consumers);
        }

        [TestMethod]
        public void GetMessage()
        {
            Messenger messenger = Messenger.GetMessenger(_option);
            Assert.IsNotNull(messenger);
        }

        [TestMethod]
        public void TestConsumer()
        {
            for (int i = 0; i < MaxSession; i++)
            {
                TestMessage message = new TestMessage()
                {
                    Id = i,
                    Message = CreateTestMessage(i)
                };
                _messenger.Send(message, FormatterType.Xml, null);
                Thread.Sleep(100);
                Assert.AreEqual(_messenger.MessageCount, 0);
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            _messenger?.Dispose();
        }


        public static string CreateTestMessage(int id)
        {
            return $"Message to session {id}.";
        }
    }

    public class TestMessage : IMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }

    public class TestConsumer : IMessageConsumer
    {
        public TestConsumer(int id)
        {
            this.SessionId = id;
        }
        public int SessionId { get; }
        public void Handle(IMessage message)
        {
            string expect = MessengeTest.CreateTestMessage(SessionId);
            Assert.AreEqual(expect, message);
        }
    }
}
