using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork
{
    /// <summary>
    /// Provides an abstraction that represents a living connection.
    /// </summary>
    public interface ILiteConnection : IDisposable
    {
        /// <summary>
        /// Gets the connection unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the connection socket.
        /// </summary>
        Socket Socket { get; }

        /// <summary>
        /// Handle an incoming packet message asynchronously.
        /// </summary>
        /// <param name="packetBuffer">Incoming packet buffer.</param>
        /// <returns>A <see cref="Task"/> that completes when finished the handle message operation.</returns>
        Task HandleMessageAsync(byte[] packetBuffer);

        /// <summary>
        /// Sends a raw <see cref="byte[]" /> buffer to the remote end point.
        /// </summary>
        /// <param name="packetBuffer">Raw packet buffer as a byte array.</param>
        void Send(byte[] packetBuffer);
    }
}
