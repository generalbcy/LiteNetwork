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

        public Guid Id { get; }

        public INetSocket Socket { get; }

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public IPacketProcessor PacketProcessor
        {
            get => this._packetProcessor;
            set
            {
                if (this.IsRunning)
                    throw new InvalidOperationException("Cannot update packet processor when server is running.");
                this._packetProcessor = value;
            }
        }

        internal INetClientConnector Connector { get; }

        protected NetClient()
        {
            this.Id = Guid.NewGuid();
            this._packetProcessor = new NetPacketProcessor();
            this.Connector = new NetClientConnector();
            this.Connector.Connected += this.OnClientConnected;
            this.Connector.Error += this.OnClientConnectionError;
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
            if (this.IsRunning)
                throw new InvalidOperationException("Client is already running");

            if (this.IsConnected)
                throw new InvalidOperationException("Client is already connected to remote.");

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
            throw new NotImplementedException();
        }

        private void OnClientConnectionError(object sender, SocketError e)
        {
            throw new NotImplementedException();
        }
    }
}
