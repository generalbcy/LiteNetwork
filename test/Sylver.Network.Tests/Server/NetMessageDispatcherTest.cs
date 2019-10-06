using Bogus;
using Moq;
using Sylver.Network.Data;
using Sylver.Network.Data.Internal;
using Sylver.Network.Server;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Mocks;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetMessageDispatcherTest
    {
        private readonly Randomizer _randomizer;
        private readonly Mock<INetServerClient> _clientMock;
        private readonly NetDataToken _netDataToken;

        public NetMessageDispatcherTest()
        {
            this._randomizer = new Randomizer();
            this._clientMock = new Mock<INetServerClient>();

            byte[] messageData = this._randomizer.Bytes(this._randomizer.Byte());
            this._netDataToken = new NetDataToken
            {
                HeaderData = this._randomizer.Bytes(sizeof(int)),
                MessageData = messageData,
                MessageSize = messageData.Length
            };
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DispatchMessageTest(bool includeHeader)
        {
            var packetProcessor = new NetPacketProcessorMock(includeHeader);
            var messageDispatcher = new NetMessageDispatcher(packetProcessor.Object);

            messageDispatcher.DispatchMessage(this._clientMock.Object, this._netDataToken);

            packetProcessor.Verify(x => x.CreatePacket(It.IsAny<byte[]>()), Times.Once());
            this._clientMock.Verify(x => x.HandleMessage(It.IsAny<INetPacketStream>()), Times.Once());
        }
    }
}
