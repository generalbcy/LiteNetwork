using System;
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
        public abstract void HandleMessage(INetPacketStream packetStream);

        /// <inheritdoc />
        public void SendPacket(INetPacketStream packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            this.Server.SendPacketTo(this, packet.Buffer);
        }

        /// <inheritdoc />
        public void SendPacketTo(INetConnection client, INetPacketStream packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            this.Server.SendPacketTo(client, packet.Buffer);
        }

        /// <inheritdoc />
        public void SendPacketTo(IEnumerable<INetConnection> clients, INetPacketStream packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            this.Server.SendPacketTo(clients, packet.Buffer);
        }

        /// <inheritdoc />
        public void SendPacketToAll(INetPacketStream packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            this.Server.SendPacketToAll(packet.Buffer);
        }
    }
}
