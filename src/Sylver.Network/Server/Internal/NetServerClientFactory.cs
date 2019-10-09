using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerClientFactory<TClient>
        where TClient : class, INetServerClient
    {
        private readonly ObjectFactory _clientFactory;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new <see cref="NetServerClientFactory{TClient}"/> instance.
        /// </summary>
        public NetServerClientFactory()
        {
            this._clientFactory = ActivatorUtilities.CreateFactory(typeof(TClient), new[]
            {
                typeof(Socket)
            });
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="acceptSocket">Accepted socket.</param>
        /// <param name="parentServer">Parent server.</param>
        /// <returns>New client.</returns>
        public TClient CreateClient(Socket acceptSocket, INetServer parentServer)
        {
            var client = this._clientFactory(this._serviceProvider, new[] { acceptSocket }) as TClient;

            if (client is NetServerClient netServerClient)
            {
                netServerClient.Server = parentServer;
            }

            return client;
        }
    }
}
