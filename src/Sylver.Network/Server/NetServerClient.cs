using System.Collections.Generic;
using System.Net.Sockets;
using Sylver.Network.Common;
using Sylver.Network.Data;

namespace Sylver.Network.Server
{
    public abstract class NetServerClient : NetConnection, INetServerClient
    {
        internal INetServer Server { get; set; }

        /// <summary>
        /// Creates and initializes a new <see cref="NetServerClient"/> instance.
        /// </summary>
        /// <param name="socketConnection">Client socket connection.</param>
        protected NetServerClient(Socket socketConnection) 
            : base(socketConnection)
        {
        }

        /// <inheritdoc />
        public abstract void HandleMessage(INetPacketStream packet);

        /// <inheritdoc />
        public void Send(INetPacketStream packet)
        {
            this.Server.SendTo(this, packet);
        }

        /// <inheritdoc />
        public void SendTo(INetConnection client, INetPacketStream packet)
        {
            this.Server.SendTo(client, packet);
        }

        /// <inheritdoc />
        public void SendTo(IEnumerable<INetConnection> clients, INetPacketStream packet)
        {
            this.Server.SendTo(clients, packet);
        }

        /// <inheritdoc />
        public void SendToAll(INetPacketStream packet)
        {
            this.Server.SendToAll(packet);
        }
    }
}
