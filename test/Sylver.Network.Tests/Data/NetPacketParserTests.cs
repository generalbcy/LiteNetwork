using Sylver.Network.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class NetPacketParserTests
    {
        private readonly IPacketProcessor _packetProcessor;
        private readonly NetPacketParser _packetParser;
        private readonly byte[] _buffer;
        private readonly byte[] _bufferHeader;
        private readonly byte[] _bufferContent;
        private readonly byte[] _bufferContentSize;
        private readonly int _messageSize;
        private readonly int _messageContentSize;
        private readonly string _messageContent;

        private readonly byte[] _invalidBuffer;

        public NetPacketParserTests()
        {
            this._packetProcessor = new NetPacketProcessor();
            this._packetParser = new NetPacketParser(this._packetProcessor);
            this._buffer = new List<int>(new[] { 16, 0, 0, 0, 12, 0, 0, 0, 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 }).Select(x => (byte)x).ToArray();
            this._bufferHeader = this._buffer.Take(this._packetProcessor.HeaderSize).ToArray();
            this._bufferContent = this._buffer.Skip(this._packetProcessor.HeaderSize).ToArray();
            this._bufferContentSize = this._bufferContent.Take(sizeof(int)).ToArray();
            this._messageSize = BitConverter.ToInt32(this._bufferHeader);
            this._messageContentSize = BitConverter.ToInt32(this._bufferContentSize) + sizeof(int); // extra int size for string length
            this._messageContent = Encoding.UTF8.GetString(this._bufferContent.Skip(sizeof(int)).ToArray());

            this._invalidBuffer = new List<int>(new[] { 255, 255, 255, 255, 12, 0, 0, 0, 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 }).Select(x => (byte)x).ToArray();
        }

        [Theory]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        public void ParseIncomingData(int bytesTransfered)
        {
            var token = new NetDataToken();

            for (int i = 0; !token.IsMessageComplete; i++)
            {
                byte[] incomingBuffer = this._buffer.Skip(i * bytesTransfered).Take(bytesTransfered).ToArray();

                this._packetParser.ParseIncomingData(token, incomingBuffer, bytesTransfered);
            }

            Assert.Equal(this._packetProcessor.HeaderSize, token.ReceivedHeaderBytesCount);
            Assert.Equal(this._packetProcessor.HeaderSize, token.HeaderData.Length);
            Assert.Equal(this._bufferHeader, token.HeaderData);
            Assert.NotNull(token.MessageSize);
            Assert.Equal(this._messageSize, token.MessageSize);
            Assert.Equal(this._messageContentSize, token.ReceivedMessageBytesCount);
            Assert.True(token.IsMessageComplete);
            Assert.Equal(this._bufferContent, token.MessageData);
            Assert.Equal(this._messageContent, Encoding.UTF8.GetString(token.MessageData.Skip(sizeof(int)).ToArray()));

            token.Reset();

            Assert.Null(token.HeaderData);
            Assert.Null(token.MessageData);
            Assert.Null(token.MessageSize);
            Assert.Equal(0, token.ReceivedHeaderBytesCount);
            Assert.Equal(0, token.ReceivedMessageBytesCount);
            Assert.False(token.IsMessageComplete);
        }

        [Fact]
        public void ParseIncomingData_WithInvalidSize()
        {
            var token = new NetDataToken();

            Assert.Throws<InvalidOperationException>(() => this._packetParser.ParseIncomingData(token, this._invalidBuffer, 32));
        }
    }
}
