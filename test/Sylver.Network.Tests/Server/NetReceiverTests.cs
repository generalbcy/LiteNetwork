using Moq;
using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Server.Mocks;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetReceiverTests
    {
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;
        private readonly NetServerReceiver<CustomClient> _serverReceiver;
        private readonly CustomClientMock _clientMock;

        public NetReceiverTests()
        {
            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            this._server = new NetServerMock<CustomClient>(this._serverConfiguration);
            this._serverReceiver = new NetServerReceiver<CustomClient>(this._server.Object);
            this._clientMock = new CustomClientMock();
        }

        [Fact]
        public void StartReceivingData_Test()
        {
            this._clientMock.SocketMock.ConfigureReceiveResult(false);
            this._serverReceiver.StartReceivingData(this._clientMock.Object);

            this._clientMock.SocketMock.VerifyReceive(It.IsAny<SocketAsyncEventArgs>(), Times.Once());
        }
    }
}
