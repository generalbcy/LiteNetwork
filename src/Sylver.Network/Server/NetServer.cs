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

        /// <inheritdoc />
        public NetServerConfiguration ServerConfiguration { get; }

        /// <summary>
        /// Creates a new <see cref="NetServer{TUser}"/> instance.
        /// </summary>
        public NetServer(NetServerConfiguration configuration)
        {
            this.ServerConfiguration = configuration;
            this._packetProcessor = new NetPacketProcessor();
            this._clientFactory = new NetServerClientFactory<TClient>();
            this._clients = new ConcurrentDictionary<Guid, TClient>();
            this._acceptor = new NetServerAcceptor<TClient>(this);
            this._acceptor.OnClientAccepted += this.OnClientAccepted;

            this._receiver = new NetServerReceiver<TClient>(this);

        }

        /// <inheritdoc />
        public void Start()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Server is already running.");

            this.Socket = new NetSocket(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.Socket.Bind(NetHelper.CreateIpEndPoint(this.ServerConfiguration.Host, this.ServerConfiguration.Port));
            this.Socket.Listen(this.ServerConfiguration.Backlog);

            this.IsRunning = true;
            this._acceptor.StartAccept();
            this.OnStart();
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
            this.OnStop();
        }

        /// <inheritdoc />
        public void DisconnectClient(Guid clientId)
        {
            if (!this._clients.TryRemove(clientId, out TClient client))
            {
                // TODO: error; cannot find client by id.
                return;
            }

            this.OnClientDisconnected(client);
            client.Dispose();
        }

        /// <summary>
        /// Triggers a child logic when the server has been started successfuly.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Triggers a child logic when the server has been stopped.
        /// </summary>
        protected virtual void OnStop() { }

        /// <summary>
        /// Triggers a child logic when a new client connects to the server.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnClientConnected(TClient client) { }

        /// <summary>
        /// Triggers a child logic when a client disconnects from the server.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnClientDisconnected(TClient client) { }

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

            this.OnClientConnected(newClient);
            this._receiver.StartReceivingData(newClient);
        }
    }
}
