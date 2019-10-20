using Sylver.Network.Common;
using Sylver.Network.Infrastructure;
using System;
using System.Buffers;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal sealed class NetClientReceiver : NetReceiver
    {
        private readonly INetClient _client;
        private readonly SocketAsyncEventArgs _socketAsyncEvent;

        /// <summary>
        /// Creates a new <see cref="NetClientReceiver"/> instance.
        /// </summary>
        /// <param name="client">Client.</param>
        public NetClientReceiver(INetClient client)
            : base(client.PacketProcessor)
        {
            this._client = client;
            this._socketAsyncEvent = new SocketAsyncEventArgs();
            this._socketAsyncEvent.Completed += this.OnCompleted;
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            ArrayPool<byte>.Shared.Return(socketAsyncEvent.Buffer, true);

            socketAsyncEvent.SetBuffer(null, 0, 0);
            socketAsyncEvent.UserToken = null;
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            int receiveBufferLength = this._client.ClientConfiguration.BufferSize;
            this._socketAsyncEvent.SetBuffer(ArrayPool<byte>.Shared.Rent(receiveBufferLength), 0, receiveBufferLength);

            return this._socketAsyncEvent;
        }

        /// <inheritdoc />
        protected override void OnDisconnected(INetUser client)
        {
            Console.WriteLine("Disconnected from server.");
        }

        /// <inheritdoc />
        protected override void OnError(INetUser client, SocketError socketError)
        {
            Console.WriteLine($"Error: {socketError}");
        }
    }
}
