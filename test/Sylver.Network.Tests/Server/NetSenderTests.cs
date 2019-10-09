using Bogus;
using Moq;
using Sylver.Network.Common;
using Sylver.Network.Server.Internal;
using Sylver.Network.Tests.Server.Mocks;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetSenderTests
    {
        private readonly Randomizer _randomizer;
        private readonly NetServerSender _sender;
        private readonly CustomClientMock _clientMock;

        public NetSenderTests()
        {
            this._randomizer = new Randomizer();
            this._sender = new NetServerSender();
            this._clientMock = new CustomClientMock();
        }

        [Fact]
        public void StartAndStopSenderQueueTest()
        {
            this._sender.Start();

            Assert.True(this._sender.IsRunning);

            this._sender.Stop();

            Assert.False(this._sender.IsRunning);
        }

        [Fact]
        public async Task SendPacketTest()
        {
            byte[] message = this._randomizer.Bytes(this._randomizer.Byte());

            this._clientMock.SocketMock.ConfigureSendResult(false); // Instant send

            this._sender.Start();
            this._sender.Send(new NetMessageData(this._clientMock.Object, message));

            // Wait 1 second so the sender task can process the previous message.
            await Task.Delay(1000);

            this._clientMock.SocketMock.VerifySend(It.IsAny<SocketAsyncEventArgs>(), Times.Once());

            this._sender.Stop();
            Assert.False(this._sender.IsRunning);
        }
    }
}
