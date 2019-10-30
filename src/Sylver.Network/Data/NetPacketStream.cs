using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Sylver.Network.Data
{
    public class NetPacketStream : MemoryStream, INetPacketStream
    {
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        /// <inheritdoc />
        public NetPacketStateType State { get; }

        /// <inheritdoc />
        public bool IsEndOfStream => this.Position >= this.Length;

        /// <inheritdoc />
        public virtual byte[] Buffer => this.TryGetBuffer(out ArraySegment<byte> buffer) ? buffer.ToArray() : new byte[0];

        /// <summary>
        /// Gets the encoding used to encode strings when writing on the packet stream.
        /// </summary>
        protected virtual Encoding StringWriteEncoding => Encoding.UTF8;

        /// <summary>
        /// Gets the encoding used to decode strings when reading from the packet stream.
        /// </summary>
        protected virtual Encoding StringReadEncoding => Encoding.UTF8;

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
        public virtual T[] Read<T>(int amount)
        {
            if (this.State != NetPacketStateType.Read)
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");

            if (amount <= 0)
                throw new ArgumentException($"Amount is '{amount}' and must be grather than 0.", nameof(amount));

            Type type = typeof(T);
            var array = new T[amount];

            if (type == typeof(byte))
                array = this._reader.ReadBytes(amount) as T[];
            else
            {
                for (var i = 0; i < amount; i++)
                    array[i] = this.Read<T>();
            }

            return array;
        }

        /// <inheritdoc />
        public virtual void Write<T>(T value)
        {
            if (this.State != NetPacketStateType.Write)
                throw new InvalidOperationException($"The current packet stream is in read-only mode.");

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                this.WritePrimitive<T>(value);
            }
            else
            {
                throw new NotImplementedException($"Cannot write a {typeof(T)} value into the packet stream.");
            }
        }

        /// <summary>
        /// Read a primitive type from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type to read.</typeparam>
        /// <returns></returns>
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
                    int stringLength = this._reader.ReadInt32();
                    byte[] stringBytes = this._reader.ReadBytes(stringLength);

                    primitiveValue = this.StringReadEncoding.GetString(stringBytes);
                    break;
            }

            return (T)primitiveValue;
        }

        /// <summary>
        /// Writes a primitive type to the packet stream.
        /// </summary>
        /// <typeparam name="T">Type to write.</typeparam>
        /// <param name="value">Value to write.</param>
        private void WritePrimitive<T>(object value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                    this._writer.Write(Convert.ToByte(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.SByte:
                    this._writer.Write(Convert.ToSByte(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Boolean:
                    this._writer.Write(Convert.ToBoolean(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Char:
                    this._writer.Write(Convert.ToChar(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int16:
                    this._writer.Write(Convert.ToInt16(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt16:
                    this._writer.Write(Convert.ToUInt16(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int32:
                    this._writer.Write(Convert.ToInt32(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt32:
                    this._writer.Write(Convert.ToUInt32(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Single:
                    this._writer.Write(Convert.ToSingle(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int64:
                    this._writer.Write(Convert.ToInt64(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt64:
                    this._writer.Write(Convert.ToUInt64(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Double:
                    this._writer.Write(Convert.ToDouble(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.String:
                    {
                        string stringValue = value.ToString();

                        this._writer.Write(stringValue.Length);

                        if (stringValue.Length > 0)
                            this._writer.Write(this.StringWriteEncoding.GetBytes(stringValue));
                    }
                    break;
            }
        }
    }
}
