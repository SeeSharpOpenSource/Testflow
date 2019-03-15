using System.Runtime.Serialization;
using Testflow.EngineCore.Common;

namespace Testflow.EngineCore.Data
{
    public class HostInfo : ISerializable
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int PortNum { get; set; }
        public RuntimePlatform Platform { get; set; }

        public bool IsRemoteHost => Constants.LocalHostAddr.Equals(IpAddress);

        public HostInfo()
        {
            this.Id = -1;
            this.IpAddress = Constants.LocalHostAddr;
            this.PortNum = -1;
            this.Platform = RuntimePlatform.Clr;
        }

        public HostInfo(int id, string ipAddress, int portNum, RuntimePlatform platform)
        {
            this.Id = id;
            this.IpAddress = ipAddress;
            this.PortNum = portNum;
            this.Platform = platform;
        }

        public HostInfo(SerializationInfo info, StreamingContext context)
        {
            this.Id = (int) info.GetValue("Id", typeof (int));
            this.IpAddress = info.GetValue("IpAddress", typeof (string)) as string;
            this.PortNum = (int) info.GetValue("PortNum", typeof (int));
            this.Platform = (RuntimePlatform) info.GetValue("Platform", typeof (RuntimePlatform));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("IpAddress", IpAddress);
            info.AddValue("PortNum", PortNum);
            info.AddValue("Platform", Platform);
        }
    }
}