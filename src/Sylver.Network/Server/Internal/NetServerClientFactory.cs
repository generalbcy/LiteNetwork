using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sylver.Network.Tests")]
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
            this._clientFactory = ActivatorUtilities.CreateFactory(typeof(TClient), new[] { typeof(Socket) });
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="acceptSocket">Accepted socket.</param>
        /// <returns>New client.</returns>
        public TClient CreateClient(Socket acceptSocket)
        {
            var newClient = this._clientFactory(this._serviceProvider, new[] { acceptSocket }) as TClient;

            return newClient;
        }
    }
}
