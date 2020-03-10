using Bogus;
using Sylver.Network.Data;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Sylver.Network.Tests.Data
{
    public sealed class NetPacketSteramWriterTests
    {
        private readonly Randomizer _randomizer;

        public NetPacketSteramWriterTests()
        {
            _randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
        }

        [Fact]
        public void PacketStreamWriteNonPrimitiveTest()
        {
            using (INetPacketStream packetStream = new NetPacketStream())
            {
                Assert.Throws<NotImplementedException>(() => packetStream.Write<object>(new object()));
            }
        }

        [Fact]
        public void PacketStreamWriteWhenReadOnlyTest()
        {
            using (INetPacketStream packetStream = new NetPacketStream(_randomizer.Bytes(_randomizer.Byte())))
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Write<byte>(_randomizer.Byte()));
            }
        }

        [Fact]
        public void PacketStreamWriteByteTest()
        {
            byte byteValue = _randomizer.Byte();

            PacketStreamWritePrimitive(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamWriteSByteTest()
        {
            sbyte sbyteValue = _randomizer.SByte();

            PacketStreamWritePrimitive<sbyte>(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamWriteBooleanTest()
        {
            bool booleanValue = _randomizer.Bool();

            PacketStreamWritePrimitive<bool>(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamWriteCharTest()
        {
            char charValue = _randomizer.Char(max: 'z');

            PacketStreamWritePrimitive<char>(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamWriteShortTest()
        {
            short shortValue = _randomizer.Short();

            PacketStreamWritePrimitive<short>(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamWriteUShortTest()
        {
            ushort ushortValue = _randomizer.UShort();

            PacketStreamWritePrimitive<ushort>(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamWriteIntTest()
        {
            int intValue = _randomizer.Int();

            PacketStreamWritePrimitive<int>(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamWriteUIntTest()
        {
            uint uintValue = _randomizer.UInt();

            PacketStreamWritePrimitive<uint>(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamWriteLongTest()
        {
            long longValue = _randomizer.Long();

            PacketStreamWritePrimitive<long>(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamWriteULongTest()
        {
            ulong ulongValue = _randomizer.ULong();

            PacketStreamWritePrimitive<ulong>(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamWriteFloatTest()
        {
            float floatValue = _randomizer.Float();

            PacketStreamWritePrimitive<float>(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamWriteDoubleTest()
        {
            double doubleValue = _randomizer.Double();

            PacketStreamWritePrimitive<double>(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamWriteStringTest()
        {
            string stringValue = new Faker().Lorem.Sentence();
            byte[] stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            PacketStreamWritePrimitive<string>(stringValue, stringValueArray, adjustBuffer: false);
        }

        private void PacketStreamWritePrimitive<T>(T valueToWrite, byte[] expectedByteArray, bool adjustBuffer = true)
        {
            using (INetPacketStream packetStream = new NetPacketStream())
            {
                Assert.Equal(NetPacketStateType.Write, packetStream.State);

                packetStream.Write(valueToWrite);

                byte[] adjustedBuffer = adjustBuffer ? expectedByteArray.Take(Marshal.SizeOf<T>()).ToArray() : expectedByteArray;

                Assert.Equal(adjustedBuffer, packetStream.Buffer);
            }
        }
    }
}
