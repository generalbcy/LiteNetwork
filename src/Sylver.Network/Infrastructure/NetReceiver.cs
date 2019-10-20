using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Data.Internal;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Sylver.Network.Infrastructure
{
    internal abstract class NetReceiver : INetReceiver
    {
        /// <inheritdoc />
        public event EventHandler<object> ReceivedComplete;

        private readonly NetPacketParser _packetParser;
        private readonly INetMessageDispatcher _messageDispatcher;
        private bool _disposedValue;

        public NetReceiver(IPacketProcessor packetProcessor)
        {
            this._packetParser = new NetPacketParser(packetProcessor);
            this._messageDispatcher = new NetMessageDispatcher(packetProcessor);
        }

        /// <inheritdoc />
        public void Start(INetUser clientConnection)
        {
            SocketAsyncEventArgs socketAsyncEvent = this.GetSocketEvent();
            socketAsyncEvent.UserToken = new NetToken(clientConnection);

            this.ReceiveData(clientConnection, socketAsyncEvent);
        }

        /// <summary>
        /// Receive data from a client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ReceiveData(INetConnection client, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (!client.Socket.ReceiveAsync(socketAsyncEvent))
                this.ProcessReceive(socketAsyncEvent.UserToken as NetToken, socketAsyncEvent);
        }

        /// <summary>
        /// Process the received data.
        /// </summary>
        /// <param name="clientToken">Client token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        private void ProcessReceive(NetToken clientToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (socketAsyncEvent == null)
                throw new ArgumentNullException("Cannot receive data from a null socket event.", nameof(socketAsyncEvent));

            if (socketAsyncEvent.BytesTransferred > 0)
            {
                if (socketAsyncEvent.SocketError == SocketError.Success)
                {
                    this._packetParser.ParseIncomingData(clientToken, socketAsyncEvent.Buffer, socketAsyncEvent.BytesTransferred);

                    if (clientToken.IsMessageComplete)
                    {
                        this._messageDispatcher.DispatchMessage(clientToken.Client, clientToken);
                        clientToken.Reset();
                    }

                    this.ReceiveData(clientToken.Client, socketAsyncEvent);
                }
                else
                {
                    this.OnError(clientToken.Client, socketAsyncEvent.SocketError);
                }
            }
            else
            {
                this.ClearSocketEvent(socketAsyncEvent);
                this.OnDisconnected(clientToken.Client);
            }
        }

        /// <summary>
        /// Fired when a receive operation has completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        protected internal void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (e.LastOperation == SocketAsyncOperation.Receive)
                this.ProcessReceive(e.UserToken as NetToken, e);
            else
                throw new InvalidOperationException($"Unknown '{e.LastOperation}' socket operation in receiver.");
        }

        /// <summary>
        /// Dispose the net receiver resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the net receiver.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                this._disposedValue = true;
            }
        }

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> for the receive operation.
        /// </summary>
        /// <returns></returns>
        protected abstract SocketAsyncEventArgs GetSocketEvent();

        /// <summary>
        /// Clears an used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async vent arguments to clear.</param>
        protected abstract void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent);

        /// <summary>
        /// Client has been disconnected.
        /// </summary>
        /// <param name="client">Disconnected client.</param>
        protected abstract void OnDisconnected(INetUser client);

        /// <summary>
        /// A socket error has occured.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="socketError">Socket error.</param>
        protected abstract void OnError(INetUser client, SocketError socketError);
    }
}
