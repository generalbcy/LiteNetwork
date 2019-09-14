using Sylver.Network.Common;
using Sylver.Network.Server.Internal;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sylver.Network.Tests")]
namespace Sylver.Network.Server
{
    /// <summary>
    /// Provides an abstraction to manage a TCP server.
    /// </summary>
    /// <typeparam name="TUser">The user type that the server will manage.</typeparam>
    public abstract class NetServer<TUser> : NetConnection, INetServer
        where TUser : class, INetServerClient
    {
        private readonly ConcurrentDictionary<Guid, TUser> _clients;
        private readonly NetServerAcceptor<TUser> _acceptor;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates a new <see cref="NetServer{TUser}"/> instance.
        /// </summary>
        public NetServer()
        {
            this._acceptor = new NetServerAcceptor<TUser>(this);
        }

        /// <inheritdoc />
        public void Start()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Server is already running.");

            if (this.Socket != null)
            {
                this.Socket.Dispose();
                this.Socket = null;
            }

            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.Socket.Bind(NetHelper.CreateIpEndPoint("127.0.0.1", 4444));
            this.Socket.Listen(50);

            this.IsRunning = true;
            this._acceptor.StartAccept();
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        internal void CreateClient(SocketAsyncEventArgs acceptedSocketEvent)
        {

            //SocketAsyncEventArgs readSocket = (this._receiver as NetServerReceiver).ReadPool.Pop();

            //if (readSocket == null)
            //    return;

            //var client = Activator.CreateInstance(typeof(T), acceptedSocketEvent.AcceptSocket) as T;

            //if (client is NetServerClient netServerClient)
            //    netServerClient.Server = this;

            //if (this._clients.ContainsKey(client.Id))
            //    throw new InvalidProgramException($"Client {client.Id} already exists in client list.");

            //this._clients.TryAdd(client.Id, client);
            //this.OnClientConnected(client);

            //readSocket.UserToken = client;
            //if (!acceptedSocketEvent.AcceptSocket.ReceiveAsync(readSocket))
            //    this._receiver.Receive(readSocket);
        }
    }
}
