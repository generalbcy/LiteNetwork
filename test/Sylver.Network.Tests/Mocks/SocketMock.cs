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

        public void ConfigureAcceptResult(bool result)
        {
            this._socketMock.Setup(x => x.AcceptAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x => result);
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

        public void ConfigureReceiveResult(bool result)
        {
            this._socketMock.Setup(x => x.ReceiveAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x => result);
        }

        public bool ReceiveAsync(SocketAsyncEventArgs socketAsyncEvent)
        {
            return this._socketMock.Object.ReceiveAsync(socketAsyncEvent);
        }

        public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            this._socketMock.Object.SetSocketOption(optionLevel, optionName, optionValue);
        }

        public void VerifyAccept(SocketAsyncEventArgs socketAsyncEvent)
        {
            this._socketMock.Verify(x => x.AcceptAsync(socketAsyncEvent), Times.Once);
        }

        public void VerifyBind(EndPoint localEP)
        {
            this._socketMock.Verify(x => x.Bind(localEP), Times.Once);
        }

        public void VerifyDispose()
        {
            this._socketMock.Verify(x => x.Dispose(), Times.Once);
        }

        public void VerifyGetSocket()
        {
            this._socketMock.Verify(x => x.GetSocket(), Times.Once);
        }

        public void VerifyListen(int backlog)
        {
            this._socketMock.Verify(x => x.Listen(backlog), Times.Once);
        }

        public void VerifyReceive(SocketAsyncEventArgs socketAsyncEvent)
        {
            this._socketMock.Verify(x => x.ReceiveAsync(socketAsyncEvent), Times.Once);
        }

        public void VerifySetSocketOptions(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            this._socketMock.Verify(x => x.SetSocketOption(optionLevel, optionName, optionValue), Times.Once);
        }
    }
}
