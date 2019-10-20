using Sylver.Network.Common;
using System;

namespace Sylver.Network.Infrastructure
{
    internal interface INetReceiver : IDisposable
    {
        event EventHandler<object> ReceivedComplete;

        void Start(INetUser clientConnection);
    }
}
