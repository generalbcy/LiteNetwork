using Bogus;
using Moq;
using Sylver.Network.Data;
using Sylver.Network.Data.Internal;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Mocks;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetMessageDispatcherTest
    {
        private readonly Randomizer _randomizer;
        private readonly CustomClientMock _clientMock;
        private readonly NetDataToken _netDataToken;

        public NetMessageDispatcherTest()
        {
            this._randomizer = new Randomizer();
            this._clientMock = new CustomClientMock();

            byte[] messageData = this._randomizer.Bytes(this._randomizer.Byte());
            this._netDataToken = new NetDataToken
            {
                HeaderData = BitConverter.GetBytes((long)messageData.Length),
                MessageData = messageData,
                MessageSize = messageData.Length
            };
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DispatchMessageTest(bool includeHeader)
        {
            var packetProcessor = new CustomNetPacketProcessor(includeHeader);
            var messageDispatcher = new NetMessageDispatcher(packetProcessor);

            messageDispatcher.DispatchMessage(this._clientMock.Object, this._netDataToken);

            // Wait for 1 second so the dispatch task can start correctly.
            await Task.Delay(1000); 

            this._clientMock.Verify(x => x.HandleMessage(It.IsAny<INetPacketStream>()), Times.Once());
        }
    }
}
