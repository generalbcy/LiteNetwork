using Sylver.Network.Data;
using Sylver.Network.Data.Internal;

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
