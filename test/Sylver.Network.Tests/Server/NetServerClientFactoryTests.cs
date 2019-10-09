using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerClientFactoryTests
    {
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerClientFactory<CustomClient> _factory;
        private readonly NetServerMock<CustomClient> _serverMock;
        private readonly Socket _clientSocket;

        public NetServerClientFactoryTests()
        {
            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            this._factory = new NetServerClientFactory<CustomClient>();
            this._serverMock = new NetServerMock<CustomClient>(this._serverConfiguration);
            this._clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        [Fact]
        public void CreateClientTest()
        {
            CustomClient newClient = this._factory.CreateClient(this._clientSocket, this._serverMock.Object);

            Assert.NotNull(newClient);
            Assert.NotNull(newClient.Server);
            Assert.NotNull(newClient.Socket);
            Assert.IsType<CustomClient>(newClient);
            Assert.NotEqual(Guid.Empty, newClient.Id);
            Assert.Equal(this._clientSocket, newClient.Socket.GetSocket());
            Assert.Equal(this._serverMock.Object, newClient.Server);
        }

        [Fact]
        public void CreateAndDisposeClient()
        {
            CustomClient newClient = this._factory.CreateClient(this._clientSocket, this._serverMock.Object);

            Assert.NotNull(newClient);
            Assert.NotNull(newClient.Socket);
            newClient.Dispose();
            Assert.Null(newClient.Socket);
        }
    }
}
