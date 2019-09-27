using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Common;
using Sylver.Network.Data;
using System;
using System.Buffers;
using System.Linq;
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
        public void StartReceivingData(TClient client, NetToken<TClient> existingToken = null)
        {
            SocketAsyncEventArgs socketAsyncEvent = this.GetSocketEventFromPool();
            socketAsyncEvent.UserToken = existingToken ?? new NetToken<TClient>(client);

            this.ReceiveData(client, socketAsyncEvent);
        }

        private void ReceiveData(TClient client, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (!client.Socket.ReceiveAsync(socketAsyncEvent))
            {
                this.ProcessReceive(socketAsyncEvent.UserToken as NetToken<TClient>, socketAsyncEvent);
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
                // TODO: receive data
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
            socketAsyncEvent.Completed += this.OnCompleted;

            return socketAsyncEvent;
        }

        /// <summary>
        /// Returns the socket event into the event pool.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async event to return.</param>
        private void ReturnSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            ArrayPool<byte>.Shared.Return(socketAsyncEvent.Buffer, true);
            this._readPool.Return(socketAsyncEvent);
        }

        private void ParseIncomingData(NetToken<TClient> token, SocketAsyncEventArgs socketAsyncEvent, IPacketProcessor packetProcessor)
        {
        }
    }
}
