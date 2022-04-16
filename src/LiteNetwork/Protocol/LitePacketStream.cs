using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteNetwork.Protocol
{
    /// <summary>
    /// Provides a basic and buil-in packet stream mechanism.
    /// </summary>
    public class LitePacketStream : MemoryStream
    {
        private readonly BinaryReader _reader = null!;
        private readonly BinaryWriter _writer = null!;

        public LitePacketMode Mode { get; }

        public bool IsEndOfStream => Position >= Length;
        
        public virtual byte[] Buffer => TryGetBuffer(out ArraySegment<byte> buffer) ? buffer.ToArray() : Array.Empty<byte>();

        /// <summary>
        /// Gets the encoding used to encode strings when writing on the packet stream.
        /// </summary>
        /// <remarks>
        /// Default encoding is UTF-8.
        /// </remarks>
        protected virtual Encoding WriteEncoding => Encoding.UTF8;

        /// <summary>
        /// Gets the encoding used to decode strings when reading from the packet stream.
        /// </summary>
        /// <remarks>
        /// Default encoding is UTF-8.
        /// </remarks>
        protected virtual Encoding ReadEncoding => Encoding.UTF8;

        /// <summary>
        /// Creates and initializes a new <see cref="LitePacketStream"/> instance in write-only mode.
        /// </summary>
        public LitePacketStream()
        {
            _writer = new BinaryWriter(this, WriteEncoding);
            Mode = LitePacketMode.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="LitePacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public LitePacketStream(byte[] buffer)
            : base(buffer, 0, buffer?.Length ?? throw new ArgumentNullException(nameof(buffer)), false, true)
        {
            _reader = new BinaryReader(this, ReadEncoding);
            Mode = LitePacketMode.Read;
        }

        /// <summary>
        /// Reads the next byte from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns>The next byte read from the current stream.</returns>
        public virtual new byte ReadByte() => Read<byte>();

        /// <summary>
        /// Reads the next signed byte from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns></returns>
        public virtual sbyte ReadSByte() => Read<sbyte>();

        /// <summary>
        /// Reads the next character from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns></returns>
        public virtual char ReadChar() => Read<char>();

        /// <summary>
        /// Reads the next boolean value from the current packet stream and advances the current position of the packet stream by one byte.
        /// </summary>
        /// <returns>True if non-zero; false otherwise.</returns>
        public virtual bool ReadBoolean() => Read<bool>();

        /// <summary>
        /// Reads a 2-bytes signed numeric value from the current packet stream and advances the current position of the packet stream by two bytes.
        /// </summary>
        /// <returns></returns>
        public virtual short ReadInt16() => Read<short>();

        /// <summary>
        /// Reads a 2-bytes unsigned numeric value from the current packet stream and advances the current position of the packet stream by two bytes.
        /// </summary>
        /// <returns></returns>
        public virtual ushort ReadUInt16() => Read<ushort>();

        /// <summary>
        /// Reads a 4-bytes signed numeric value from the current packet stream and advances the current position of the packet stream by four bytes.
        /// </summary>
        /// <returns></returns>
        public virtual int ReadInt32() => Read<int>();

        /// <summary>
        /// Reads a 4-bytes unsigned numeric value from the current packet stream and advances the current position of the packet stream by four bytes.
        /// </summary>
        /// <returns></returns>
        public virtual uint ReadUInt32() => Read<uint>();

        /// <summary>
        /// Reads a 8-bytes signed numeric value from the current packet stream and advances the current position of the packet stream by eight bytes.
        /// </summary>
        /// <returns></returns>
        public virtual long ReadInt64() => Read<long>();

        /// <summary>
        /// Reads a 8-bytes unsigned numeric value from the current packet stream and advances the current position of the packet stream by eight bytes.
        /// </summary>
        /// <returns></returns>
        public virtual ulong ReadUInt64() => Read<ulong>();

        /// <summary>
        /// Reads a 4-bytes floating numeric value from the current packet stream and advances the current position of the packet stream by four bytes.
        /// </summary>
        /// <returns></returns>
        public virtual float ReadSingle() => Read<float>();

        /// <summary>
        /// Reads a 8-bytes floating numeric value from the current packet stream and advances the current position of the packet stream by eight bytes.
        /// </summary>
        /// <returns></returns>
        public virtual double ReadDouble() => Read<double>();

        /// <summary>
        /// Reads a string from the current packet stream, where the first 4-byte represents the string length, then advances the current position of the packet stream by the length of the string plus four bytes.
        /// </summary>
        /// <returns></returns>
        public virtual string ReadString() => Read<string>();

        /// <summary>
        /// Reads a given amount of bytes from the current packet stream and advances the current position of the packet stream by the number of read bytes.
        /// </summary>
        /// <param name="count">Byte amount to read.</param>
        /// <returns></returns>
        public virtual byte[] ReadBytes(int count) => _reader.ReadBytes(count);

        /// <summary>
        /// Reads a <typeparamref name="T"/> value from the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value to read.</typeparam>
        /// <returns>The value read and converted to the type.</returns>
        public virtual T Read<T>()
        {
            if (Mode != LitePacketMode.Read)
            {
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");
            }

            Type type = typeof(T);

            if (type.IsPrimitive || type == typeof(string))
            {
                return ReadPrimitive<T>(type);
            }

            throw new NotImplementedException($"Cannot read a {typeof(T)} value from the packet stream.");
        }

        /// <summary>
        /// Reads an array of <typeparamref name="T"/> value from the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="amount">Amount to read.</param>
        /// <returns>An array of type <typeparamref name="T"/>.</returns>
        public virtual T[] Read<T>(int amount)
        {
            if (Mode != LitePacketMode.Read)
            {
                throw new InvalidOperationException($"The current packet stream is in write-only mode.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException($"Amount is '{amount}' and must be grather than 0.", nameof(amount));
            }

            if (typeof(T) == typeof(byte))
            {
                return _reader.ReadBytes(amount) as T[] ?? throw new IOException("An error occurred while reading a packet stream.");
            }

            var array = new T[amount];

            for (var i = 0; i < amount; i++)
            {
                array[i] = Read<T>();
            }

            return array;
        }


        /// <summary>
        /// Writes a signed byte to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteSByte(sbyte value) => Write(value);

        /// <summary>
        /// Writes a character to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteChar(char value) => Write(value);

        /// <summary>
        /// Writes a boolean value to the current packet stream and advances the current packet stream position by one byte.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteBoolean(bool value) => Write(value);

        /// <summary>
        /// Writes a 2-bytes signed numeric value to the current packet stream and advances the current packet stream position byte two bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteInt16(short value) => Write(value);

        /// <summary>
        /// Writes a 2-bytes unsigned numeric value to the current packet stream and advances the current packet stream position byte two bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteUInt16(ushort value) => Write(value);

        /// <summary>
        /// Writes a 4-bytes signed numeric value to the current packet stream and advances the current packet stream position byte four bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteInt32(int value) => Write(value);

        /// <summary>
        /// Writes a 4-bytes signed numeric value to the current packet stream and advances the current packet stream position byte four bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteUInt32(uint value) => Write(value);

        /// <summary>
        /// Writes a 4-bytes floating numeric value to the current packet stream and advances the current packet stream position byte four bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteSingle(float value) => Write(value);

        /// <summary>
        /// Writes a 8-bytes floating numeric value to the current packet stream and advances the current packet stream position byte eight bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteDouble(double value) => Write(value);

        /// <summary>
        /// Writes a 8-bytes floating signed value to the current packet stream and advances the current packet stream position byte eight bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteInt64(long value) => Write(value);

        /// <summary>
        /// Writes a 8-bytes unsigned numeric value to the current packet stream and advances the current packet stream position byte eight bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteUInt64(ulong value) => Write(value);

        /// <summary>
        /// Writes a string to the current packet stream, where the first four bytes represents the string length and advances the current packet stream position by the string length + four bytes.
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteString(string value) => Write(value);

        /// <summary>
        /// Writes a given byte array to the current packet stream and advances the current packet stream position by the array length.
        /// </summary>
        /// <param name="values"></param>
        public virtual void WriteBytes(byte[] values) => _writer.Write(values);

        /// <summary>
        /// Writes a <typeparamref name="T"/> value in the packet stream.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">Value to write in the packet stream.</param>
        public virtual void Write<T>(T value)
        {
            if (Mode != LitePacketMode.Write)
            {
                throw new InvalidOperationException($"The current packet stream is in read-only mode.");
            }

            if (value is null)
            {
                throw new ArgumentNullException("Cannot write a null value into the packet stream.");
            }

            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                WritePrimitive<T>(value);
            }
            else
            {
                throw new NotImplementedException($"Cannot write a {typeof(T)} value into the packet stream.");
            }
        }

        /// <summary>
        /// Read a primitive type from the packet stream.
        /// </summary>
        /// <param name="type">Type to read.</param>
        /// <typeparam name="T">Type to read.</typeparam>
        /// <returns></returns>
        private T ReadPrimitive<T>(Type type)
        {
            object primitiveValue = Type.GetTypeCode(type) switch
            {
                TypeCode.Byte => _reader.ReadByte(),
                TypeCode.SByte => _reader.ReadSByte(),
                TypeCode.Boolean => _reader.ReadBoolean(),
                TypeCode.Char => _reader.ReadChar(),
                TypeCode.Int16 => _reader.ReadInt16(),
                TypeCode.UInt16 => _reader.ReadUInt16(),
                TypeCode.Int32 => _reader.ReadInt32(),
                TypeCode.UInt32 => _reader.ReadUInt32(),
                TypeCode.Single => _reader.ReadSingle(),
                TypeCode.Double => _reader.ReadDouble(),
                TypeCode.Int64 => _reader.ReadInt64(),
                TypeCode.UInt64 => _reader.ReadUInt64(),
                TypeCode.String => InternalReadString(),
                _ => throw new NotImplementedException(),
            };

            return (T)primitiveValue;
        }

        /// <summary>
        /// Reads a string from the current packet stream.
        /// </summary>
        /// <returns></returns>
        private string InternalReadString()
        {
            int stringLength = ReadInt32();
            byte[] stringBytes = ReadBytes(stringLength);

            return ReadEncoding.GetString(stringBytes);
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
                    _writer.Write(Convert.ToByte(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.SByte:
                    _writer.Write(Convert.ToSByte(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Boolean:
                    _writer.Write(Convert.ToBoolean(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Char:
                    _writer.Write(Convert.ToChar(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int16:
                    _writer.Write(Convert.ToInt16(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt16:
                    _writer.Write(Convert.ToUInt16(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int32:
                    _writer.Write(Convert.ToInt32(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt32:
                    _writer.Write(Convert.ToUInt32(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Single:
                    _writer.Write(Convert.ToSingle(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Int64:
                    _writer.Write(Convert.ToInt64(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.UInt64:
                    _writer.Write(Convert.ToUInt64(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.Double:
                    _writer.Write(Convert.ToDouble(value, CultureInfo.DefaultThreadCurrentCulture));
                    break;
                case TypeCode.String:
                    {
                        if (value != null)
                        {
                            string stringValue = value.ToString();

                            _writer.Write(stringValue.Length);

                            if (stringValue.Length > 0)
                            {
                                _writer.Write(WriteEncoding.GetBytes(stringValue));
                            }
                        }
                    }
                    break;
            }
        }
    }
}
