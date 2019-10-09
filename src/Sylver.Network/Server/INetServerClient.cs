using Sylver.Network.Common;
using Sylver.Network.Data;
using System.Collections.Generic;

namespace Sylver.Network.Server
{
    public interface INetServerClient : INetConnection
    {
        /// <summary>
        /// Handle an incoming packet.
        /// </summary>
        /// <param name="packet">Incoming packet stream.</param>
        void HandleMessage(INetPacketStream packetStream);

        /// <summary>
        /// Sends a packet to this client.
        /// </summary>
        /// <param name="packet">Packet stream</param>
        void SendPacket(INetPacketStream packet);

        /// <summary>
        /// Sends a packet to an other client.
        /// </summary>
        /// <param name="client">Other client connection</param>
        /// <param name="packet">Packet to send.</param>
        void SendPacketTo(INetConnection client, INetPacketStream packet);

        /// <summary>
        /// Sends a packet to collection of clients.
        /// </summary>
        /// <param name="clients">Collection of clients</param>
        /// <param name="packet">Packet to send.</param>
        void SendPacketTo(IEnumerable<INetConnection> clients, INetPacketStream packet);

        /// <summary>
        /// Sends a packet to every connected clients on the server.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        void SendPacketToAll(INetPacketStream packet);
    }
}
