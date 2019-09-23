using System;
using System.Linq.Expressions;
using Moq;
using Sylver.Network.Server;

namespace Sylver.Network.Tests.Mocks
{
    public sealed class NetServerMock<TClient> : Mock<NetServer<TClient>>
        where TClient : class, INetServerClient
    {
        public NetServerMock(params object[] args)
            : base(args)
        {
        }

        public NetServerMock(MockBehavior behavior)
            : base(behavior)
        {
        }

        public NetServerMock(MockBehavior behavior, params object[] args)
            : base(behavior, args)
        {
        }

        public NetServerMock(Expression<Func<NetServer<TClient>>> newExpression, MockBehavior behavior = MockBehavior.Loose)
            : base(newExpression, behavior)
        {
        }
    }
}
