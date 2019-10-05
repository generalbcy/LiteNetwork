using Bogus;
using Sylver.Network.Data;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class NetPacketStreamTests
    {
        private readonly Randomizer _randomizer;

        public NetPacketStreamTests()
        {
            this._randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
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

            this.PacketStreamReadTest<string>(stringValue, stringValueArray);
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

        private void PacketStreamReadTest<T>(T expectedValue, byte[] valueAsBytes)
        {
            using (var packetStream = new NetPacketStream(valueAsBytes))
            {
                var readValue = packetStream.Read<T>();

                Assert.Equal(expectedValue, readValue);
            }
        }

        public static byte[] GetBytes(decimal decimalValue) 
            => decimal.GetBits(decimalValue).SelectMany(x => BitConverter.GetBytes(x)).ToArray();
    }
}
