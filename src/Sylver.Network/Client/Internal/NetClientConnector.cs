using Sylver.Network.Common;
using System;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal sealed class NetClientConnector : INetClientConnector
    {
        private readonly INetClient _client;
        private readonly SocketAsyncEventArgs _socketAsyncEvent;

        private uint _connectionRetryAttempts;

        /// <inheritdoc />
        public event EventHandler Connected;

        /// <inheritdoc />
        public event EventHandler<SocketError> Error;

        /// <summary>
        /// Creates a new <see cref="NetClientConnector"/> instance.
        /// </summary>
        public NetClientConnector(INetClient client)
        {
            this._client = client;
            this._socketAsyncEvent = new SocketAsyncEventArgs
            {
                DisconnectReuseSocket = true
            };
            this._socketAsyncEvent.Completed += this.OnCompleted;
        }

        /// <inheritdoc />
        public void Connect()
        {
            this._connectionRetryAttempts = 0;
            this._socketAsyncEvent.RemoteEndPoint = NetHelper.CreateIpEndPoint(this._client.ClientConfiguration.Host, this._client.ClientConfiguration.Port);

            this.InternalConnect();
        }

        /// <summary>
        /// Dispose the net client connector.
        /// </summary>
        public void Dispose()
        {
            this._socketAsyncEvent.Dispose();
        }

        /// <summary>
        /// Connects to the remote end point.
        /// </summary>
        private void InternalConnect()
        {
            if (!this._client.Socket.ConnectAsync(this._socketAsyncEvent))
            {
                this.OnCompleted(this, this._socketAsyncEvent);
            }
        }

        /// <summary>
        /// Retry the connection if failed.
        /// </summary>
        private void RetryConnect()
        {
            this._connectionRetryAttempts++;

            switch (this._client.ClientConfiguration.Retry.Mode)
            {
                case NetClientRetryOption.Infinite:
                    this.InternalConnect();
                    break;
                case NetClientRetryOption.Limited:
                    if (this._connectionRetryAttempts < this._client.ClientConfiguration.Retry.MaxAttempts)
                    {
                        this.InternalConnect();
                    }
                    break;
                case NetClientRetryOption.OneTime:
                    if (this._connectionRetryAttempts > 1)
                    {
                        this.Error?.Invoke(this, SocketError.HostUnreachable);
                    }
                    else
                    {
                        this.InternalConnect();
                    }
                    break;
            }
        }

        private void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.LastOperation == SocketAsyncOperation.Connect)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // TODO: create event args ?
                        this.Connected?.Invoke(this, null);
                    }
                    else
                    {
                        this.Error?.Invoke(this, e.SocketError);
                        this.RetryConnect();
                    }
                }
            }
            catch (StackOverflowException)
            {
                // TODO: create event args for the errors
                this.Error?.Invoke(this, SocketError.HostUnreachable);
            }
        }
    }
}
