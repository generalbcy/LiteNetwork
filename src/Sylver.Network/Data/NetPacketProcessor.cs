using System;
using System.Linq;

namespace Sylver.Network.Data
{
    /// <summary>
    /// Default Sylver packet processor.
    /// </summary>
    internal sealed class NetPacketProcessor : IPacketProcessor
    {
        /// <inheritdoc />
        public int HeaderSize => sizeof(int);

        /// <inheritdoc />
        public bool IncludeHeader => false;

        /// <inheritdoc />
        public int GetMessageLength(byte[] buffer)
        {
            return BitConverter.ToInt32(BitConverter.IsLittleEndian
                ? buffer.Take(this.HeaderSize).ToArray()
                : buffer.Take(this.HeaderSize).Reverse().ToArray(), 0);
        }
    }
}
