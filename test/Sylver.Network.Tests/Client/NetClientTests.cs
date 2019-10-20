using Moq;
using Sylver.Network.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Client
{
    public sealed class NetClientTests
    {
        private readonly Mock<NetClient> _client;

        public static IEnumerable<object[]> InvalidConfigurations => new List<object[]>
        {
            new object[] { new NetClientConfiguration("127.0.0.1", 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("127.0.0.1", int.MinValue), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("347.SD.23S", 4444), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration(null, 0), typeof(ArgumentException) },
            new object[] { new NetClientConfiguration("33432.3433.265.45445", 0), typeof(ArgumentException) },
        };

        public NetClientTests()
        {
            var configuration = new NetClientConfiguration("127.0.0.1", 4444);
            this._client = new Mock<NetClient>(configuration);
        }

        [Theory]
        [MemberData(nameof(InvalidConfigurations))]
        public void NetClientConnectWithInvalidConfigurationTest(NetClientConfiguration configuration, Type exceptionType)
        {
            
        }
    }
}
