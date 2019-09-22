using Moq;
using Sylver.Network.Common;
using System.Net;
using System.Net.Sockets;

namespace Sylver.Network.Tests.Mocks
{
    public sealed class SocketMock : INetSocket
    {
        private readonly Mock<INetSocket> _socketMock;

        public SocketMock()
        {
            this._socketMock = new Mock<INetSocket>();
        }

        public bool AcceptAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return this._socketMock.Object.AcceptAsync(socketAsyncEvent);
        }

        public void Bind(EndPoint localEP)
        {
            this._socketMock.Object.Bind(localEP);
        }

        public void Dispose()
        {
            this._socketMock.Object.Dispose();
        }

        public Socket GetSocket()
        {
            return this._socketMock.Object.GetSocket();
        }

        public void Listen(int backlog)
        {
            this._socketMock.Object.Listen(backlog);
        }

        public bool ReceiveAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return this._socketMock.Object.ReceiveAsync(socketAsyncEvent);
        }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            this._socketMock.Object.SetSocketOption(optionLevel, optionName, optionValue);
        }
    }
}
