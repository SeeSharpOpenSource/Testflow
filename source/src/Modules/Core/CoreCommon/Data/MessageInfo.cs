using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Data
{
    public class MessageInfo : IMessage
    {
        public int Id { get; }

        public string Name { get; set; }

        public object[] Param { get; set; }

        public MessageInfo(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}