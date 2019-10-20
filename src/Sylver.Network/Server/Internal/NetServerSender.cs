using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Infrastructure;
using System.Net.Sockets;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerSender : NetSender
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _writePool;

        /// <summary>
        /// Creates a new <see cref="NetServerSender"/> instance.
        /// </summary>
        public NetServerSender()
        {
            this._writePool = ObjectPool.Create<SocketAsyncEventArgs>();
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent()
        {
            SocketAsyncEventArgs socketAsyncEvent = this._writePool.Get();

            socketAsyncEvent.Completed += this.OnSendCompleted;

            return socketAsyncEvent;
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            socketAsyncEvent.Completed -= this.OnSendCompleted;
            socketAsyncEvent.SetBuffer(null, 0, 0);

            this._writePool.Return(socketAsyncEvent);
        }
    }
}
