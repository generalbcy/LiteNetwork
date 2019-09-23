using Sylver.Network.Common;
using System;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerReceiver<TClient> : IDisposable
        where TClient : class, INetServerClient
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;

        /// <summary>
        /// Creates a new <see cref="NetServerReceiver{TClient}"/> instance.
        /// </summary>
        /// <param name="bufferManager">Buffer manager.</param>
        /// <param name="maxClients">Maximum amount of clients.</param>
        public NetServerReceiver(BufferManager bufferManager, int maxClients)
        {
            this._readPool = new ObjectPool<SocketAsyncEventArgs>();

            for (int i = 0; i < maxClients; i++)
            {
                var socketEvent = new SocketAsyncEventArgs();
                socketEvent.Completed += this.OnCompleted;

                bufferManager.SetBuffer(socketEvent);

                this._readPool.Push(socketEvent);
            }
        }

        /// <summary>
        /// Initialize the client and starts receving data.
        /// </summary>
        /// <param name="client">Client to initialize.</param>
        public void InitializeClientAndStartReceiving(TClient client)
        {
            var token = new NetToken<TClient>(client);
            SocketAsyncEventArgs socketEvent = this._readPool.Pop();

            socketEvent.UserToken = token;

            this.StartReceiving(token, socketEvent);
        }

        /// <summary>
        /// Starts receving data.
        /// </summary>
        /// <param name="clientToken">Client token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void StartReceiving(NetToken<TClient> clientToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (!clientToken.Client.Socket.ReceiveAsync(socketAsyncEvent))
            {
                this.ProcessReceive(clientToken, socketAsyncEvent);
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
                // TODO: execute framing algorithm
            }
            else
            {
                // TODO: close connection
            }
        }

        /// <summary>
        /// Disposes the receiver resources.
        /// </summary>
        public void Dispose()
        {
            this._readPool.Dispose();
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
    }
}
