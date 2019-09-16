using System.Net.Sockets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sylver.Network.Tests")]
namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerClientFactory
    {
        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <typeparam name="TUser">Client type.</typeparam>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        /// <returns>New client.</returns>
        public TUser CreateClient<TUser>(SocketAsyncEventArgs socketAsyncEvent)
            where TUser : class, INetServerClient
        {
            return default;
        }
    }
}
