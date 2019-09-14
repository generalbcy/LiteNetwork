using Sylver.Network.Common;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

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

        public bool IsRunning { get; private set; }

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
            this.StartAccept();
        }

        /// <inheritdoc />
        public void Stop()
        {
        }

        // Accept

        private void StartAccept()
        {
            var socketAcceptorAsync = new SocketAsyncEventArgs();
            socketAcceptorAsync.Completed += this.OnSocketCompleted;

            this.Accept(socketAcceptorAsync);
        }

        private void OnSocketCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Accept:
                        this.ProcessAccept(e);
                        //this._acceptor.ProcessAccept(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        //this._receiver.Receive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        //this._sender.SendOperationCompleted(e);
                        break;
                    default: throw new InvalidOperationException("Unexpected SocketAsyncOperation.");
                }
            }
            catch (Exception)
            {
                //this.OnError(exception);
            }
        }

        private void Accept(SocketAsyncEventArgs e)
        {
            if (e.AcceptSocket != null)
                e.AcceptSocket = null;

            if (!this.Socket.AcceptAsync(e))
                this.ProcessAccept(e);
        }

        /// <summary>
        /// Process the accept connection async operation.
        /// </summary>
        /// <param name="e"></param>
        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                this.CreateClient(e);
                this.Accept(e);
            }
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
