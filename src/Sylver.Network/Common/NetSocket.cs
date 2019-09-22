using System.Net;
using System.Net.Sockets;

namespace Sylver.Network.Common
{
    public class NetSocket : INetSocket
    {
        private readonly Socket _socket;

        /// <summary>
        /// Creates a new <see cref="NetSocket"/> instance.
        /// </summary>
        /// <param name="socket">Socket.</param>
        public NetSocket(Socket socket)
        {
            this._socket = socket;
        }

        /// <inheritdoc />
        public Socket GetSocket() => this._socket;

        /// <inheritdoc />
        public bool AcceptAsync(SocketAsyncEventArgs socketAsyncEvent) => this._socket.AcceptAsync(socketAsyncEvent);

        /// <inheritdoc />
        public bool ReceiveAsync(SocketAsyncEventArgs socketAsyncEvent) => this._socket.ReceiveAsync(socketAsyncEvent);

        /// <inheritdoc />
        public void Listen(int backlog) => this._socket.Listen(backlog);

        /// <inheritdoc />
        public void Bind(EndPoint localEP) => this._socket.Bind(localEP);

        /// <inheritdoc />
        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue) 
            => this._socket.SetSocketOption(optionLevel, optionName, optionValue);

        /// <inheritdoc />
        public void Dispose() => this._socket.Dispose();
    }
}
