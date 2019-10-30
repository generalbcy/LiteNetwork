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
            this._randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void PacketStreamReadNonPrimitiveTest()
        {
            using (var packetStream = new NetPacketStream(this._randomizer.Bytes(this._randomizer.Byte())))
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
            byte byteValue = this._randomizer.Byte();

            this.PacketStreamReadTest<byte>(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamReadSByteTest()
        {
            sbyte sbyteValue = this._randomizer.SByte();

            this.PacketStreamReadTest<sbyte>(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamReadBooleanTest()
        {
            bool booleanValue = this._randomizer.Bool();

            this.PacketStreamReadTest<bool>(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamReadCharTest()
        {
            char charValue = this._randomizer.Char(max: 'z');

            this.PacketStreamReadTest<char>(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamReadShortTest()
        {
            short shortValue = this._randomizer.Short();

            this.PacketStreamReadTest<short>(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamReadUShortTest()
        {
            ushort ushortValue = this._randomizer.UShort();

            this.PacketStreamReadTest<ushort>(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamReadIntTest()
        {
            int intValue = this._randomizer.Int();

            this.PacketStreamReadTest<int>(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamReadUIntTest()
        {
            uint uintValue = this._randomizer.UInt();

            this.PacketStreamReadTest<uint>(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamReadLongTest()
        {
            long longValue = this._randomizer.Long();

            this.PacketStreamReadTest<long>(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamReadULongTest() 
        {
            ulong ulongValue = this._randomizer.ULong();

            this.PacketStreamReadTest<ulong>(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamReadFloatTest()
        {
            float floatValue = this._randomizer.Float();

            this.PacketStreamReadTest<float>(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamReadDoubleTest()
        {
            double doubleValue = this._randomizer.Double();

            this.PacketStreamReadTest<double>(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamReadStringTest()
        {
            string stringValue = new Faker().Lorem.Sentence();
            byte[] stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            this.PacketStreamReadTest<string>(stringValue, stringValueArray, adjustBuffer: false);
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
            using (var packetStream = new NetPacketStream(this._randomizer.Bytes(this._randomizer.Byte())))
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Read<object>(this._randomizer.Byte(min: 1)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void PacketStreamReadArrayWithInvalidAmountTest(int amount)
        {
            using (var packetStream = new NetPacketStream(this._randomizer.Bytes(this._randomizer.Byte())))
            {
                Assert.Throws<ArgumentException>(() => packetStream.Read<byte>(amount));
            }
        }

        [Fact]
        public void PacketStreamReadArrayWhenWriteOnlyTest()
        {
            using (var packetStream = new NetPacketStream())
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Read<byte>(this._randomizer.Byte(min: 1)));
            }
        }

        [Fact]
        public void PacketStreamReadByteArrayTest()
        {
            var buffer = this._randomizer.Bytes(this._randomizer.Byte());
            int amount = this._randomizer.Int(min: 1, max: buffer.Length);
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
