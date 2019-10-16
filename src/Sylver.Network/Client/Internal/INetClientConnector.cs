using System;
using System.Net.Sockets;

namespace Sylver.Network.Client.Internal
{
    internal interface INetClientConnector : IDisposable
    {
        event EventHandler Connected;

        event EventHandler<SocketError> Error;

        void Connect();
    }
}
