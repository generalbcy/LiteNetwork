using Moq;
using Sylver.Network.Client;
using Sylver.Network.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sylver.Network.Tests.Client
{
    public sealed class NetClientTests
    {
        private readonly NetSocketMock _socketMock;
        private readonly Mock<NetClient> _client;

        public static IEnumerable<object[]> InvalidConfigurations => new List<object[]>
        {
            new object[] { new NetClientConfiguration("127.0.0.1", 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("127.0.0.1", int.MinValue), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("347.SD.23S", 4444), typeof(AggregateException) },
            new object[] { new NetClientConfiguration(null, 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("33432.3433.265.45445", 0), typeof(ArgumentException) },
            new object[] { null, typeof(ArgumentNullException) }
        };

        public NetClientTests()
        {
            var configuration = new NetClientConfiguration("127.0.0.1", 4444);

            this._socketMock = new NetSocketMock();
            this._client = new Mock<NetClient>(configuration);
            this._client.SetupGet(x => x.Socket).Returns(this._socketMock);
        }

        [Theory]
        [MemberData(nameof(InvalidConfigurations))]
        public void NetClientConnectWithInvalidConfigurationTest(NetClientConfiguration configuration, Type exceptionType)
        {
            using (INetClient client = new NetClient(configuration))
            {
                Assert.Throws(exceptionType, () => client.Connect());
            }
        }

        [Fact]
        public async Task NetClientConnectWithValidConfigurationTest()
        {
            this._socketMock.ConfigureConnectResult(false); // instance connection

            Assert.False(this._client.Object.IsConnected);
            this._client.Object.Connect();
            await Task.Delay(1000).ConfigureAwait(false); // Fake connection
            Assert.True(this._client.Object.IsConnected);
        }
    }
}
