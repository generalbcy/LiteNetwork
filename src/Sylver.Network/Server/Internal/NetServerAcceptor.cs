using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    /// <summary>
    /// Accepts the clients into the server.
    /// </summary>
    internal class NetServerAcceptor
    {
        private readonly INetServer _server;

        /// <summary>
        /// Gets the acceptor socket async event arguments.
        /// </summary>
        internal SocketAsyncEventArgs SocketEvent { get; }

        /// <summary>
        /// Event fired when a client is accepted.
        /// </summary>
        public event EventHandler<SocketAsyncEventArgs> OnClientAccepted;

        /// <summary>
        /// Creates a new <see cref="NetServerAcceptor{TUser}"/> instance.
        /// </summary>
        /// <param name="server"></param>
        public NetServerAcceptor(INetServer server)
        {
            this._server = server;
            this.SocketEvent = new SocketAsyncEventArgs();
            this.SocketEvent.Completed += this.OnSocketCompleted;
        }

        /// <summary>
        /// Starts accepting clients into the server.
        /// </summary>
        public void StartAccept()
        {
            if (this.SocketEvent.AcceptSocket != null)
                this.SocketEvent.AcceptSocket = null;

            if (!this._server.Socket.AcceptAsync(this.SocketEvent))
            {
                this.ProcessAccept(this.SocketEvent);
            }
        }

        /// <summary>
        /// Process a new connected client.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ProcessAccept(SocketAsyncEventArgs socketAsyncEvent)
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
