using System.Net.Sockets;
using Sylver.Network.Common;
using Sylver.Network.Server;

namespace Sylver.Network.Tests.Server
{
    public class CustomClient : NetConnection, INetServerClient
    {
        public CustomClient(Socket socketConnection)
            : base(socketConnection)
        {
        }
    }
}
