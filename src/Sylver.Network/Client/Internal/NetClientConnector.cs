using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Sylver.Network.Client.Internal
{
    internal sealed class NetClientConnector : INetClientConnector
    {
        private readonly SocketAsyncEventArgs _socketAsyncEvent;

        public event EventHandler Connected;
        public event EventHandler<SocketError> Error;

        /// <summary>
        /// Creates a new <see cref="NetClientConnector"/> instance.
        /// </summary>
        public NetClientConnector()
        {
            this._socketAsyncEvent = new SocketAsyncEventArgs();
        }

        /// <inheritdoc />
        public void Connect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose the net client connector.
        /// </summary>
        public void Dispose()
        {
            this._socketAsyncEvent.Dispose();
        }
    }
}
