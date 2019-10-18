using Sylver.Network.Client.Internal;
using Sylver.Network.Common;
using Sylver.Network.Data;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Sylver.Network.Client
{
    public class NetClient : INetClient
    {
        private bool _disposedValue;
        private IPacketProcessor _packetProcessor;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public INetSocket Socket { get; }

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public NetClientConfiguration ClientConfiguration { get; protected set; }

        /// <inheritdoc />
        public IPacketProcessor PacketProcessor
        {
            get => this._packetProcessor;
            set
            {
                if (this.IsConnected)
                    throw new InvalidOperationException("Cannot update packet processor when the client is already connected.");
                this._packetProcessor = value;
            }
        }

        internal INetClientConnector Connector { get; }

        protected NetClient()
        {
            this.Id = Guid.NewGuid();
            this.Socket = new NetSocket(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            this.Connector = new NetClientConnector(this);
            this.Connector.Connected += this.OnClientConnected;
            this.Connector.Error += this.OnClientConnectionError;
            this._packetProcessor = new NetPacketProcessor();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    this.Connector.Dispose();
                }

                this._disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="NetClient"/> resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Client is already connected to remote.");

            if (this.ClientConfiguration.Port <= 0)
                throw new ArgumentException($"Invalid port number '{this.ClientConfiguration.Port}' in configuration.", nameof(this.ClientConfiguration.Port));

            if (NetHelper.BuildIPAddress(this.ClientConfiguration.Host) == null)
                throw new ArgumentException($"Invalid host address '{this.ClientConfiguration.Host}' in configuration", nameof(this.ClientConfiguration.Host));

            if (this.ClientConfiguration.BufferSize <= 0)
                throw new ArgumentException($"Invalid buffer size '{this.ClientConfiguration.BufferSize}' in configuration.", nameof(this.ClientConfiguration.BufferSize));

            this.Connector.Connect();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual void HandleMessage(INetPacketStream packet)
        {
            // Nothing to do. Must be override in child classes.
        }

        /// <inheritdoc />
        public void SendMessage(INetPacketStream packet)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnConnected() { }

        protected virtual void OnDisconnected() { }

        private void OnClientConnected(object sender, EventArgs e)
        {
            this.OnConnected();
        }

        private void OnClientConnectionError(object sender, SocketError e)
        {
            Console.WriteLine($"Connection error: {e.ToString()}");
        }
    }
}
