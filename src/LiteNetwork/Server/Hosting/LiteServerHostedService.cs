using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteNetwork.Server.Hosting
{
    /// <summary>
    /// Define a basic <see cref="IHostedService"/> to use with <see cref="LiteServer{TUser}"/>.
    /// </summary>
    /// <typeparam name="TLiteServerUser">The user that will be use by the sever.</typeparam>
    internal class LiteServerHostedService<TLiteServerUser> : IHostedService
        where TLiteServerUser : LiteServerUser
    {
        private readonly LiteServer<TLiteServerUser> _server;

        /// <summary>
        /// Creates a new <see cref="LiteServerHostedService{TLiteServerUser}"/> with the given server.
        /// </summary>
        /// <param name="server">Server to host.</param>
        public LiteServerHostedService(LiteServer<TLiteServerUser> server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server), $"Failed to inject server for user type: {typeof(TLiteServerUser).Name}");
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _server.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _server.StopAsync(cancellationToken);
        }
    }
}
