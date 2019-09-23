using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    /// <summary>
    /// Accepts the clients into the server.
    /// </summary>
    /// <typeparam name="TUser">User type handled by the server.</typeparam>
    internal sealed class NetServerAcceptor<TUser>
        where TUser : class, INetServerClient
    {
        private readonly NetServer<TUser> _server;
        private readonly SocketAsyncEventArgs _socketEvent;

        /// <summary>
        /// Event fired when a client is accepted.
        /// </summary>
        public event EventHandler<SocketAsyncEventArgs> OnClientAccepted;

        /// <summary>
        /// Creates a new <see cref="NetServerAcceptor{TUser}"/> instance.
        /// </summary>
        /// <param name="server"></param>
        public NetServerAcceptor(NetServer<TUser> server)
        {
            this._server = server;
            this._socketEvent = new SocketAsyncEventArgs();
            this._socketEvent.Completed += this.OnSocketCompleted;
        }

        /// <summary>
        /// Starts accepting clients into the server.
        /// </summary>
        public void StartAccept()
        {
            if (this._socketEvent.AcceptSocket != null)
                this._socketEvent.AcceptSocket = null;

            if (!this._server.Socket.AcceptAsync(this._socketEvent))
            {
                this.ProcessAccept(this._socketEvent);
            }
        }

        /// <summary>
        /// Process a new connected client.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        public void ProcessAccept(SocketAsyncEventArgs socketAsyncEvent)
        {
            if (socketAsyncEvent.SocketError == SocketError.Success)
            {
                this.OnClientAccepted?.Invoke(this, socketAsyncEvent);
                this.StartAccept();
            }
        }

        /// <summary>
        /// Fired when a socket operation has completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        [ExcludeFromCodeCoverage]
        private void OnSocketCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e.LastOperation == SocketAsyncOperation.Accept)
            {
                this.ProcessAccept(e);
            }
            else
            {
                throw new InvalidOperationException($"Unknown '{e.LastOperation}' socket operation in accecptor.");
            }
        }
    }
}
