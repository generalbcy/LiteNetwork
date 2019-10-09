using System;
using System.Net.Sockets;

namespace Sylver.Network.Common
{
    public abstract class NetConnection : INetConnection, IDisposable
    {
        private bool _disposedValue;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public virtual INetSocket Socket { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="NetConnection"/> instance.
        /// </summary>
        protected internal NetConnection()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="NetConnection"/> instance with a socket.
        /// </summary>
        /// <param name="socketConnection">Socket</param>
        protected NetConnection(Socket socketConnection)
        {
            this.Id = Guid.NewGuid();
            
            if (socketConnection != null)
                this.Socket = new NetSocket(socketConnection);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the <see cref="NetConnection"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                this.Socket.Dispose();
                this.Socket = null;

                this._disposedValue = true;
            }
        }
    }
}
