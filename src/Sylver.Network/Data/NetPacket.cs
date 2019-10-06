using System.IO;

namespace Sylver.Network.Data
{
    /// <summary>
    /// Represents a Sylver built-in packet.
    /// </summary>
    public sealed class NetPacket : NetPacketStream
    {
        public const int HeaderSize = sizeof(int);

        /// <inheritdoc />
        public override byte[] Buffer
        {
            get
            {
                if (this.State == NetPacketStateType.Write)
                {
                    long oldPosition = this.Position;

                    this.Seek(0, SeekOrigin.Begin);
                    this.Write((int)this.ContentLength);
                    this.Seek((int)oldPosition, SeekOrigin.Begin);
                }

                return base.Buffer;
            }
        }

        /// <summary>
        /// Gets the packet's content length.
        /// </summary>
        public long ContentLength => this.Length - HeaderSize;

        /// <summary>
        /// Creates a new <see cref="NetPacket"/> in write-only mode.
        /// </summary>
        public NetPacket()
        {
            this.Write(0); // Packet size
        }

        /// <summary>
        /// Creates a new <see cref="NetPacket"/> in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacket(byte[] buffer)
            : base(buffer)
        {
        }
    }
}
