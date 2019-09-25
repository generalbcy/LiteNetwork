using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Common;
using Sylver.Network.Data;
using System;
using System.Buffers;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerReceiver<TClient>
        where TClient : class, INetServerClient
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;
        private readonly NetServer<TClient> _server;

        /// <summary>
        /// Creates a new <see cref="NetServerReceiver{TClient}"/> instance.
        /// </summary>
        public NetServerReceiver(NetServer<TClient> server)
        {
            this._readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            this._server = server;
        }

        /// <summary>
        /// Initialize the client and starts receving data.
        /// </summary>
        /// <param name="client">Client to initialize.</param>
        public void StartReceiving(TClient client)
        {
            var token = new NetToken<TClient>(client);
            SocketAsyncEventArgs socketAsyncEvent = this.GetSocketEventFromPool();

            socketAsyncEvent.UserToken = token;

            if (!client.Socket.ReceiveAsync(socketAsyncEvent))
            {
                this.ProcessReceive(token, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Process the received data.
        /// </summary>
        /// <param name="clientToken">Client token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ProcessReceive(NetToken<TClient> clientToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (socketAsyncEvent == null)
            {
                throw new ArgumentNullException("Cannot receive data from a null socket event.", nameof(socketAsyncEvent));
            }

            if (socketAsyncEvent.SocketError == SocketError.Success && socketAsyncEvent.BytesTransferred > 0)
            {
                byte[] message = this.Receive(clientToken, socketAsyncEvent, this._server.PacketProcessor);

                this.ReturnSocketEvent(socketAsyncEvent);

                if (message != null)
                {
                    // TODO: dispatch messages to client.
                }

                this.StartReceiving(clientToken.Client);
            }
            else
            {
                // TODO: close connection
            }
        }

        /// <summary>
        /// Fired when a receive operation has completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        private void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                this.ProcessReceive(e.UserToken as NetToken<TClient>, e);
            }
            else
            {
                throw new InvalidOperationException($"Unknown '{e.LastOperation}' socket operation in receiver.");
            }
        }

        /// <summary>
        /// Gets a socket event from the event pool.
        /// </summary>
        /// <returns>Available <see cref="SocketAsyncEventArgs"/>.</returns>
        private SocketAsyncEventArgs GetSocketEventFromPool()
        {
            SocketAsyncEventArgs socketAsyncEvent = this._readPool.Get();

            byte[] buffer = ArrayPool<byte>.Shared.Rent(32);
            socketAsyncEvent.SetBuffer(buffer, 0, buffer.Length);

            return socketAsyncEvent;
        }

        /// <summary>
        /// Returns the socket event into the event pool.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async event to return.</param>
        private void ReturnSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            ArrayPool<byte>.Shared.Return(socketAsyncEvent.Buffer);
            this._readPool.Return(socketAsyncEvent);
        }

        private byte[] Receive(NetToken<TClient> token, SocketAsyncEventArgs socketAsyncEvent, IPacketProcessor packetProcessor)
        {
            return null;
        }
    }
}
