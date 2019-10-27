using Sylver.Network.Client.Internal;
using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Infrastructure;
using System;
using System.Net.Sockets;

namespace Sylver.Network.Client
{
    public class NetClient : NetConnection, INetClient
    {
        private IPacketProcessor _packetProcessor;

        private readonly INetClientConnector _connector;
        private readonly INetSender _sender;
        private readonly INetReceiver _receiver;

        /// <inheritdoc />
        public bool IsConnected => this.Socket.IsConnected;

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

        /// <summary>
        /// Creates a new <see cref="NetClient"/> instance without any configuration.
        /// </summary>
        public NetClient()
            : this(null)
        {
        }

        /// <summary>
        /// Creates and configures a new <see cref="NetClient"/> instance.
        /// </summary>
        /// <param name="clientConfiguration"></param>
        public NetClient(NetClientConfiguration clientConfiguration)
            : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            this.ClientConfiguration = clientConfiguration;
            this._packetProcessor = new NetPacketProcessor();
            this._connector = new NetClientConnector(this);
            this._connector.Connected += this.OnClientConnected;
            this._connector.Error += this.OnClientConnectionError;
            this._sender = new NetClientSender();
            this._receiver = new NetClientReceiver(this);
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Client is already connected to remote.");

            if (this.ClientConfiguration == null)
                throw new ArgumentNullException("Client configuration is not set.", nameof(this.ClientConfiguration));

            if (this.ClientConfiguration.Port <= 0)
                throw new ArgumentException($"Invalid port number '{this.ClientConfiguration.Port}' in configuration.", nameof(this.ClientConfiguration.Port));

            if (NetHelper.BuildIPAddress(this.ClientConfiguration.Host) == null)
                throw new ArgumentException($"Invalid host address '{this.ClientConfiguration.Host}' in configuration", nameof(this.ClientConfiguration.Host));

            if (this.ClientConfiguration.BufferSize <= 0)
                throw new ArgumentException($"Invalid buffer size '{this.ClientConfiguration.BufferSize}' in configuration.", nameof(this.ClientConfiguration.BufferSize));

            this._sender.Start();
            this._connector.Connect();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            this._sender.Stop();
            this.Socket.GetSocket().Disconnect(true);
            this.OnDisconnected();
        }

        /// <inheritdoc />
        public void Send(INetPacketStream packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            this._sender.Send(new NetMessageData(this, packet.Buffer));
        }

        /// <inheritdoc />
        public virtual void HandleMessage(INetPacketStream packet)
        {
            // Nothing to do. Must be override in child classes.
        }

        /// <summary>
        /// Dispose the <see cref="NetClient"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this._connector.Dispose();
            this._sender.Dispose();
            this._receiver.Dispose();
            base.Dispose(disposing);
        }

        protected virtual void OnConnected() { }

        protected virtual void OnDisconnected() { }

        private void OnClientConnected(object sender, EventArgs e)
        {
            this.OnConnected();
            this._receiver.Start(this);
        }

        private void OnClientConnectionError(object sender, SocketError e)
        {
            Console.WriteLine($"Connection error: {e.ToString()}");
        }
    }
}
