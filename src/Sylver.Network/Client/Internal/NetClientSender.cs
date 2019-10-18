using Sylver.Network.Common.Internal;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal sealed class NetClientSender : NetSender
    {
        private readonly SocketAsyncEventArgs _socketAsyncEvent;

        /// <summary>
        /// Creates a new <see cref="NetClientSender"/> instance.
        /// </summary>
        public NetClientSender()
        {
            this._socketAsyncEvent = new SocketAsyncEventArgs();
            this._socketAsyncEvent.Completed += this.OnSendCompleted;
        }

        /// <inheritdoc />
        protected override void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            socketAsyncEvent.SetBuffer(null, 0, 0);
        }

        /// <inheritdoc />
        protected override SocketAsyncEventArgs GetSocketEvent() => this._socketAsyncEvent;
    }
}
