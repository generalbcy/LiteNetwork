using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Server.Internal;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Sylver.Network.Server
{
    /// <summary>
    /// Provides an abstraction to manage a TCP server.
    /// </summary>
    /// <typeparam name="TClient">The client type that the server will manage.</typeparam>
    public class NetServer<TClient> : NetConnection, INetServer
        where TClient : class, INetServerClient
    {
        private readonly NetServerConfiguration _configuration;
        private readonly ConcurrentDictionary<Guid, TClient> _clients;
        private readonly NetServerClientFactory<TClient> _clientFactory;
        private readonly NetServerAcceptor<TClient> _acceptor;
        private readonly NetServerReceiver<TClient> _receiver;

        private IPacketProcessor _packetProcessor;

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

        /// <summary>
        /// Creates a new <see cref="NetServer{TUser}"/> instance.
        /// </summary>
        public NetServer(NetServerConfiguration configuration)
        {
            this._configuration = configuration;
            this._clientFactory = new NetServerClientFactory<TClient>();
            this._clients = new ConcurrentDictionary<Guid, TClient>();
            this._acceptor = new NetServerAcceptor<TClient>(this);
            this._acceptor.OnClientAccepted += this.OnClientAccepted;

            this._receiver = new NetServerReceiver<TClient>(this);

            this._packetProcessor = new NetPacketProcessor();
        }

        /// <inheritdoc />
        public void Start()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Server is already running.");

            this.Socket = new NetSocket(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.Socket.Bind(NetHelper.CreateIpEndPoint(this._configuration.Host, this._configuration.Port));
            this.Socket.Listen(this._configuration.Backlog);

            this.IsRunning = true;
            this._acceptor.StartAccept();
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (this.Socket != null)
            {
                this.Socket.Dispose();
                this.Socket = null;
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// Fired when a client is accepted to the server.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Accepted client socket async event arguments.</param>
        private void OnClientAccepted(object sender, SocketAsyncEventArgs e)
        {
            TClient newClient = this._clientFactory.CreateClient(e.AcceptSocket);

            if (!this._clients.TryAdd(newClient.Id, newClient))
            {
                // TODO: send error.
            }

            this._receiver.StartReceiving(newClient);
        }
    }
}
