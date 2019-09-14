using Sylver.Network.Common;
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

        void Start();

        void Stop();
    }
}
