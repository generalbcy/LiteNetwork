using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Data.Internal;
using System;
using System.Threading.Tasks;

namespace Sylver.Network.Infrastructure
{
    internal sealed class NetMessageDispatcher : INetMessageDispatcher
    {
        /// <summary>
        /// Gets or sets the packet processor.
        /// </summary>
        public IPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Creates a new <see cref="NetMessageDispatcher"/> instance to dispatch messages to clients.
        /// </summary>
        /// <param name="packetProcessor">Packet processor.</param>
        public NetMessageDispatcher(IPacketProcessor packetProcessor)
        {
            this.PacketProcessor = packetProcessor;
        }

        /// <inheritdoc />
        public void DispatchMessage(INetUser client, NetDataToken token)
        {
            var bufferSize = this.PacketProcessor.IncludeHeader ? this.PacketProcessor.HeaderSize + token.MessageSize.Value : token.MessageSize.Value;
            var buffer = new byte[bufferSize];

            if (this.PacketProcessor.IncludeHeader)
            {
                Array.Copy(token.HeaderData, 0, buffer, 0, this.PacketProcessor.HeaderSize);
                Array.Copy(token.MessageData, 0, buffer, this.PacketProcessor.HeaderSize, token.MessageSize.Value);
            }
            else
                Array.Copy(token.MessageData, 0, buffer, 0, token.MessageSize.Value);

            Task.Run(() =>
            {
                using (INetPacketStream packet = this.PacketProcessor.CreatePacket(buffer))
                    client.HandleMessage(packet);
            });
        }
    }
}
