using System.Net.Sockets;
using Sylver.Network.Common;
using Sylver.Network.Data;

namespace Sylver.Network.Server
{
    public abstract class NetServerClient : NetConnection, INetServerClient
    {
        protected NetServerClient(Socket socketConnection) 
            : base(socketConnection)
        {
        }

        /// <inheritdoc />
        public abstract void HandleMessage(INetPacketStream packetStream);
    }
}
