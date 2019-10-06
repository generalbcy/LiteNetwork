using Moq;
using Sylver.Network.Data;
using System;
using System.Linq;

namespace Sylver.Network.Tests.Mocks
{
    internal sealed class CustomNetPacketProcessor : IPacketProcessor
    {
        public int HeaderSize => sizeof(long);

        public bool IncludeHeader => false;

        public int GetMessageLength(byte[] buffer)
        {
            return BitConverter.ToInt32(BitConverter.IsLittleEndian
                ? buffer.Take(this.HeaderSize).ToArray()
                : buffer.Take(this.HeaderSize).Reverse().ToArray(), 0);
        }

        public INetPacketStream CreatePacket(byte[] buffer) => new NetPacket(buffer);
    }

    internal sealed class CustomNetPacketProcessorMock : Mock<CustomNetPacketProcessor>
    {
    }
}
