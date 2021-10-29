using LiteNetwork.Client.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace LiteNetwork.Client.Hosting
{
    /// <summary>
    /// Defines a basic <see cref="IHostedService"/> to use with <see cref="LiteClient"/>
    /// </summary>
    internal class LiteClientHostedService : IHostedService
    {
        private readonly ILiteClient _client;

        /// <summary>
        /// Creates a new <see cref="LiteClientHostedService"/> with the given server.
        /// </summary>
        /// <param name="client">Client to host.</param>
        public LiteClientHostedService(ILiteClient client)
        {
            _client = client;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _client.ConnectAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _client.DisconnectAsync();
        }
    }
}
