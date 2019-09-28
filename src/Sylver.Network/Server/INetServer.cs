using Sylver.Network.Common;
using Sylver.Network.Data;
using System;

namespace Sylver.Network.Server
{
    /// <summary>
    /// Provides a mechanism to manage a TCP server.
    /// </summary>
    public interface INetServer : INetConnection, IDisposable
    {
        /// <summary>
        /// Gets a value that indicates if the server is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets or sest the server's packet processor.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        IPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Disconnects a client.
        /// </summary>
        /// <param name="clientId">Client id.</param>
        void DisconnectClient(Guid clientId);
    }
}
