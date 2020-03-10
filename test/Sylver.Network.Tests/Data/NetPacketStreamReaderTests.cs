using Bogus;
using Sylver.Network.Data;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class NetPacketStreamReaderTests
    {
        private readonly Randomizer _randomizer;

        public NetPacketStreamReaderTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void PacketStreamReadNonPrimitiveTest()
        {
            using (var packetStream = new NetPacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Read<object>());
            }
        }

        [Fact]
        public void PacketStreamReadWhenWriteOnlyTest()
        {
            using (var packetStream = new NetPacketStream())
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Read<byte>());
            }
        }

        [Fact]
        public void PacketStreamReadByteTest()
        {
            byte byteValue = _randomizer.Byte();

            PacketStreamReadTest<byte>(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamReadSByteTest()
        {
            sbyte sbyteValue = _randomizer.SByte();

            PacketStreamReadTest<sbyte>(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamReadBooleanTest()
        {
            bool booleanValue = _randomizer.Bool();

            PacketStreamReadTest<bool>(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamReadCharTest()
        {
            char charValue = _randomizer.Char(max: 'z');

            PacketStreamReadTest<char>(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamReadShortTest()
        {
            short shortValue = _randomizer.Short();

            PacketStreamReadTest<short>(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamReadUShortTest()
        {
            ushort ushortValue = _randomizer.UShort();

            PacketStreamReadTest<ushort>(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamReadIntTest()
        {
            int intValue = _randomizer.Int();

            PacketStreamReadTest<int>(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamReadUIntTest()
        {
            uint uintValue = _randomizer.UInt();

            PacketStreamReadTest<uint>(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamReadLongTest()
        {
            long longValue = _randomizer.Long();

            PacketStreamReadTest<long>(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamReadULongTest() 
        {
            ulong ulongValue = _randomizer.ULong();

            PacketStreamReadTest<ulong>(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamReadFloatTest()
        {
            float floatValue = _randomizer.Float();

            PacketStreamReadTest<float>(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamReadDoubleTest()
        {
            double doubleValue = _randomizer.Double();

            PacketStreamReadTest<double>(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamReadStringTest()
        {
            string stringValue = new Faker().Lorem.Sentence();
            byte[] stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamReadTest<string>(stringValue, stringValueArray, adjustBuffer: false);
        }

        private void PacketStreamReadTest<T>(T expectedValue, byte[] valueAsBytes, bool adjustBuffer = true)
        {
            byte[] adjustedBuffer = adjustBuffer ? valueAsBytes.Take(Marshal.SizeOf<T>()).ToArray() : valueAsBytes;

            using (INetPacketStream packetStream = new NetPacketStream(adjustedBuffer))
            {
                Assert.Equal(NetPacketStateType.Read, packetStream.State);
                Assert.False(packetStream.IsEndOfStream);

                var readValue = packetStream.Read<T>();

                Assert.Equal(expectedValue, readValue);
                Assert.True(packetStream.IsEndOfStream);
            }
        }

        [Fact]
        public void PacketStreamReadNonPrimitiveArrayTest()
        {
            using (var packetStream = new NetPacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Read<object>(_randomizer.Byte(min: 1)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void PacketStreamReadArrayWithInvalidAmountTest(int amount)
        {
            using (var packetStream = new NetPacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<ArgumentException>(() => packetStream.Read<byte>(amount));
            }
        }

        [Fact]
        public void PacketStreamReadArrayWhenWriteOnlyTest()
        {
            using (var packetStream = new NetPacketStream())
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Read<byte>(_randomizer.Byte(min: 1)));
            }
        }

        [Fact]
        public void PacketStreamReadByteArrayTest()
        {
            var buffer = _randomizer.Bytes(_randomizer.Byte());
            int amount = _randomizer.Int(min: 1, max: buffer.Length);
            byte[] expectedBuffer = buffer.Take(amount).ToArray();

            using (var packetStream = new NetPacketStream(buffer))
            {
                byte[] readBuffer = packetStream.Read<byte>(amount);

                Assert.Equal(amount, readBuffer.Length);
                Assert.Equal(expectedBuffer, readBuffer);
            }
        }
    }
}
