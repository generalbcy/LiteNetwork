using Sylver.Network.Data.Internal;

namespace Sylver.Network.Server.Internal
{
    internal interface INetMessageDispatcher
    {
        /// <summary>
        /// Dispatch a message to the given client.
        /// </summary>
        /// <param name="client">Client to dispatch the message.</param>
        /// <param name="token">Client data token containing the received data.</param>
        void DispatchMessage(INetServerClient client, NetDataToken token);
    }
}
