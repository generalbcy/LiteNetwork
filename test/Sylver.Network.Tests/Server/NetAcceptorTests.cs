using Moq;
using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Mocks;
using Sylver.Network.Tests.Server.Mocks;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetAcceptorTests
    {
        private readonly NetSocketMock _serverSocket;
        private readonly NetSocketMock _acceptedSocket;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;
        private readonly NetServerAcceptor _serverAcceptor;

        public NetAcceptorTests()
        {
            this._serverSocket = new NetSocketMock();
            this._acceptedSocket = new NetSocketMock();
            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            this._server = new NetServerMock<CustomClient>(this._serverConfiguration);
            this._serverAcceptor = new NetServerAcceptor(this._server.Object);

            this._server.SetupGet(x => x.Socket).Returns(this._serverSocket);
        }

        [Fact]
        public void StartAcceptingClientsTest()
        {
            this.ConfigureSocketAcceptAsyncResult(true);
            this._serverAcceptor.StartAccept();
            this._serverSocket.VerifyAccept(this._serverAcceptor.SocketEvent, Times.AtLeastOnce());
            Assert.Null(this._serverAcceptor.SocketEvent.AcceptSocket);
        }

        [Fact]
        public void StartAcceptingClientsTwiceTest()
        {
            this.ConfigureSocketAcceptAsyncResult(true);

            this._serverAcceptor.StartAccept();
            this._serverSocket.VerifyAccept(this._serverAcceptor.SocketEvent, Times.AtLeastOnce());
            Assert.Null(this._serverAcceptor.SocketEvent.AcceptSocket);

            this._serverAcceptor.StartAccept();
            this._serverSocket.VerifyAccept(this._serverAcceptor.SocketEvent, Times.AtLeastOnce());

            Assert.Null(this._serverAcceptor.SocketEvent.AcceptSocket);
        }

        [Fact]
        public void AcceptClient_CompletedEventFired()
        {
            this.ConfigureSocketAcceptAsyncResult(false);
            this._serverAcceptor.StartAccept();
            this._serverSocket.VerifyAccept(this._serverAcceptor.SocketEvent, Times.Once());
        }

        private void ConfigureSocketAcceptAsyncResult(bool receiveData)
        {
            this._serverSocket.SocketMock.Setup(x => x.AcceptAsync(this._serverAcceptor.SocketEvent)).Returns<SocketAsyncEventArgs>(x =>
            {
                bool instantReceive = receiveData;
                receiveData = !receiveData;

                this._serverAcceptor.SocketEvent.SocketError = SocketError.Success;
                this._serverAcceptor.SocketEvent.AcceptSocket = instantReceive ? this._acceptedSocket.GetSocket() : null;

                return !instantReceive; // returning false because "AcceptAsync()" returns false if accepted instantly
            });
        }
    }
}
