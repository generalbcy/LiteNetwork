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
            return default;
        }

        /// <inheritdoc />
        public virtual void Write<T>(T value)
        {
            throw new NotImplementedException();
        }
    }
}
