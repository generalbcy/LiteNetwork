using Sylver.Network.Data;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetToken<TClient> : NetDataToken
        where TClient : class, INetServerClient
    {
        public TClient Client { get; }

        public NetToken(TClient client)
        {
            this.Client = client;
        }
    }
}
