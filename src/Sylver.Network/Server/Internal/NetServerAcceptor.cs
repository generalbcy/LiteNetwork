using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sylver.Network.Tests")]
namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerAcceptor<TUser>
        where TUser : class, INetServerClient
    {
        private readonly NetServer<TUser> _server;
        private readonly SocketAsyncEventArgs _socketEvent;

        public NetServerAcceptor(NetServer<TUser> server)
        {
            this._server = server;
            this._socketEvent = new SocketAsyncEventArgs();
            this._socketEvent.Completed += this.OnSocketCompleted;
        }

        public void StartAccept()
        {
            if (this._socketEvent.AcceptSocket != null)
                this._socketEvent.AcceptSocket = null;

            if (!this._server.Socket.AcceptAsync(this._socketEvent))
            {
                this.ProcessAccept(this._socketEvent);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs socketAsyncEvent)
        {
            if (socketAsyncEvent.SocketError == SocketError.Success)
            {
                // TODO: Create client
                this.StartAccept();
            }
        }


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
                throw new InvalidOperationException("Unknown socket operation in server acceptor.");
            }
        }
    }
}
