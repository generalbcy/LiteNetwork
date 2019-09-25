using Sylver.Network.Data;
using System;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class DefaultPacketProcessorTests
    {
        private readonly IPacketProcessor _packetProcessor;

        public DefaultPacketProcessorTests()
        {
            this._packetProcessor = new NetPacketProcessor();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(23)]
        [InlineData(0x4A)]
        [InlineData(0)]
        [InlineData(-1)]
        public void ParsePacketHeaderTest(int headerValue)
        {
            byte[] headerBuffer = BitConverter.GetBytes(headerValue);
            int packetSize = this._packetProcessor.GetMessageLength(headerBuffer);

            Assert.Equal(headerValue, packetSize);
        }
    }
}
