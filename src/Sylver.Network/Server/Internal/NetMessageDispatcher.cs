using Sylver.Network.Data;
using Sylver.Network.Data.Internal;
using System;
using System.Threading.Tasks;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetMessageDispatcher : INetMessageDispatcher
    {
        private readonly IPacketProcessor _packetProcessor;

        /// <summary>
        /// Creates a new <see cref="NetMessageDispatcher"/> instance to dispatch messages to clients.
        /// </summary>
        /// <param name="packetProcessor">Packet processor.</param>
        public NetMessageDispatcher(IPacketProcessor packetProcessor)
        {
            this._packetProcessor = packetProcessor;
        }

        /// <inheritdoc />
        public void DispatchMessage(INetServerClient client, NetDataToken token)
        {
            Task.Run(() =>
            {
                int bufferSize = this._packetProcessor.IncludeHeader ? this._packetProcessor.HeaderSize + token.MessageSize.Value : token.MessageSize.Value;
                var buffer = new byte[bufferSize];

                if (this._packetProcessor.IncludeHeader)
                {
                    Array.Copy(token.HeaderData, 0, buffer, 0, this._packetProcessor.HeaderSize);
                    Array.Copy(token.MessageData, 0, buffer, this._packetProcessor.HeaderSize, token.MessageSize.Value);
                }
                else
                {
                    Array.Copy(token.MessageData, 0, buffer, 0, token.MessageSize.Value);
                }

                using (INetPacketStream packet = this._packetProcessor.CreatePacket(buffer))
                {
                    client.HandleMessage(packet);
                }
            });
        }
    }
}
