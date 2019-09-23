using Moq;
using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetAcceptorTests
    {
        private readonly NetSocketMock _serverSocket;
        private readonly NetSocketMock _acceptedSocket;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;
        private readonly NetServerAcceptor<CustomClient> _serverAcceptor;

        public NetAcceptorTests()
        {
            this._serverSocket = new NetSocketMock();
            this._acceptedSocket = new NetSocketMock();
            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444, 50, 100, 128);
            this._server = new NetServerMock<CustomClient>(this._serverConfiguration);
            this._serverAcceptor = new NetServerAcceptor<CustomClient>(this._server.Object);

            this._server.SetupGet(x => x.Socket).Returns(this._serverSocket);
        }

        [Fact]
        public void StartAcceptingClientsTest()
        {
            bool receiveData = true;
            this._serverSocket.SocketMock.Setup(x => x.AcceptAsync(It.IsAny<SocketAsyncEventArgs>())).Returns<SocketAsyncEventArgs>(x =>
            {
                bool instantReceive = receiveData == true;
                receiveData = false;

                x.SocketError = SocketError.Success;
                x.AcceptSocket = this._acceptedSocket.GetSocket();

                return !instantReceive; // returning false because "AcceptAsync()" returns false if accepted instantly
            });

            this._serverAcceptor.StartAccept();
        }
    }
}
