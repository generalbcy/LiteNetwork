using System;
using System.IO;

namespace Sylver.Network.Data
{
    public class NetPacketStream : MemoryStream, INetPacketStream
    {
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        /// <inheritdoc />
        public NetPacketStateType State { get; }

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in write-only mode.
        /// </summary>
        public NetPacketStream()
        {
            this._writer = new BinaryWriter(this);
            this.State = NetPacketStateType.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacketStream(byte[] buffer)
            : base(buffer, 0, buffer.Length, false, true)
        {
            this._reader = new BinaryReader(this);
            this.State = NetPacketStateType.Read;
        }

        /// <inheritdoc />
        public virtual T Read<T>()
        {
            if (this.State != NetPacketStateType.Read)
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                return this.ReadPrimitive<T>();
            }

            throw new NotImplementedException($"Cannot read a {typeof(T)} value from the packet stream.");
        }

        /// <inheritdoc />
        public virtual void Write<T>(T value)
        {
            if (this.State != NetPacketStateType.Write)
                throw new InvalidOperationException($"The current packet stream is in read-only mode.");

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                this.WritePrimitive(value);
            }

            throw new NotImplementedException($"Cannot write a {typeof(T)} value into the packet stream.");
        }

        private T ReadPrimitive<T>()
        {
            object primitiveValue = default;

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                    primitiveValue = this._reader.ReadByte();
                    break;
                case TypeCode.SByte:
                    primitiveValue = this._reader.ReadSByte();
                    break;
                case TypeCode.Boolean:
                    primitiveValue = this._reader.ReadBoolean();
                    break;
                case TypeCode.Char:
                    primitiveValue = this._reader.ReadChar();
                    break;
                case TypeCode.Int16:
                    primitiveValue = this._reader.ReadInt16();
                    break;
                case TypeCode.UInt16:
                    primitiveValue = this._reader.ReadUInt16();
                    break;
                case TypeCode.Int32:
                    primitiveValue = this._reader.ReadInt32();
                    break;
                case TypeCode.UInt32:
                    primitiveValue = this._reader.ReadUInt32();
                    break;
                case TypeCode.Single:
                    primitiveValue = this._reader.ReadSingle();
                    break;
                case TypeCode.Int64:
                    primitiveValue = this._reader.ReadInt64();
                    break;
                case TypeCode.UInt64:
                    primitiveValue = this._reader.ReadUInt64();
                    break;
                case TypeCode.Double:
                    primitiveValue = this._reader.ReadDouble();
                    break;
                case TypeCode.String:
                    primitiveValue = new string(this._reader.ReadChars(count: this._reader.ReadInt32()));
                    break;
            }

            return (T)primitiveValue;
        }

        private void WritePrimitive<T>(T value)
        {
            switch (value)
            {
            }
        }
    }
}
