using Sylver.Network.Common;
using Sylver.Network.Data;

namespace Sylver.Network.Server
{
    public interface INetServerClient : INetConnection
    {
        void HandleMessage(INetPacketStream packetStream);
    }
}
