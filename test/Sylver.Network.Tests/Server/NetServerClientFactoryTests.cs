using Sylver.Network.Server.Internal;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerClientFactoryTests
    {
        private readonly NetServerClientFactory<CustomClient> _factory;

        public NetServerClientFactoryTests()
        {
            this._factory = new NetServerClientFactory<CustomClient>();
        }

        [Fact]
        public void CreateClientTest()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            CustomClient newClient = this._factory.CreateClient(socket);

            Assert.NotNull(newClient);
            Assert.IsType<CustomClient>(newClient);
            Assert.Equal(socket, newClient.Socket.GetSocket());
        }
    }
}
