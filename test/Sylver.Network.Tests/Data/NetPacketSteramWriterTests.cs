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
            this._randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
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
            using (INetPacketStream packetStream = new NetPacketStream(this._randomizer.Bytes(this._randomizer.Byte())))
            {
                Assert.Throws<InvalidOperationException>(() => packetStream.Write<byte>(this._randomizer.Byte()));
            }
        }

        [Fact]
        public void PacketStreamWriteByteTest()
        {
            byte byteValue = this._randomizer.Byte();

            this.PacketStreamWritePrimitive(byteValue, BitConverter.GetBytes(byteValue));
        }

        [Fact]
        public void PacketStreamWriteSByteTest()
        {
            sbyte sbyteValue = this._randomizer.SByte();

            this.PacketStreamWritePrimitive<sbyte>(sbyteValue, BitConverter.GetBytes(sbyteValue));
        }

        [Fact]
        public void PacketStreamWriteBooleanTest()
        {
            bool booleanValue = this._randomizer.Bool();

            this.PacketStreamWritePrimitive<bool>(booleanValue, BitConverter.GetBytes(booleanValue));
        }

        [Fact]
        public void PacketStreamWriteCharTest()
        {
            char charValue = this._randomizer.Char(max: 'z');

            this.PacketStreamWritePrimitive<char>(charValue, BitConverter.GetBytes(charValue));
        }

        [Fact]
        public void PacketStreamWriteShortTest()
        {
            short shortValue = this._randomizer.Short();

            this.PacketStreamWritePrimitive<short>(shortValue, BitConverter.GetBytes(shortValue));
        }

        [Fact]
        public void PacketStreamWriteUShortTest()
        {
            ushort ushortValue = this._randomizer.UShort();

            this.PacketStreamWritePrimitive<ushort>(ushortValue, BitConverter.GetBytes(ushortValue));
        }

        [Fact]
        public void PacketStreamWriteIntTest()
        {
            int intValue = this._randomizer.Int();

            this.PacketStreamWritePrimitive<int>(intValue, BitConverter.GetBytes(intValue));
        }

        [Fact]
        public void PacketStreamWriteUIntTest()
        {
            uint uintValue = this._randomizer.UInt();

            this.PacketStreamWritePrimitive<uint>(uintValue, BitConverter.GetBytes(uintValue));
        }

        [Fact]
        public void PacketStreamWriteLongTest()
        {
            long longValue = this._randomizer.Long();

            this.PacketStreamWritePrimitive<long>(longValue, BitConverter.GetBytes(longValue));
        }

        [Fact]
        public void PacketStreamWriteULongTest()
        {
            ulong ulongValue = this._randomizer.ULong();

            this.PacketStreamWritePrimitive<ulong>(ulongValue, BitConverter.GetBytes(ulongValue));
        }

        [Fact]
        public void PacketStreamWriteFloatTest()
        {
            float floatValue = this._randomizer.Float();

            this.PacketStreamWritePrimitive<float>(floatValue, BitConverter.GetBytes(floatValue));
        }

        [Fact]
        public void PacketStreamWriteDoubleTest()
        {
            double doubleValue = this._randomizer.Double();

            this.PacketStreamWritePrimitive<double>(doubleValue, BitConverter.GetBytes(doubleValue));
        }

        [Fact]
        public void PacketStreamWriteStringTest()
        {
            string stringValue = new Faker().Lorem.Sentence();
            byte[] stringValueArray = BitConverter.GetBytes(stringValue.Length).Concat(Encoding.UTF8.GetBytes(stringValue)).ToArray();

            this.PacketStreamWritePrimitive<string>(stringValue, stringValueArray, adjustBuffer: false);
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
