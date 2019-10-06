using Sylver.Network.Data.Internal;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetToken<TClient> : NetDataToken
        where TClient : class, INetServerClient
    {
        /// <summary>
        /// Gets or sets the client attached to the current token.
        /// </summary>
        public TClient Client { get; }

        /// <summary>
        /// Creates a new <see cref="NetToken{TClient}"/> instance.
        /// </summary>
        /// <param name="client">Client attached to this token.</param>
        public NetToken(TClient client)
        {
            this.Client = client;
        }
    }
}
