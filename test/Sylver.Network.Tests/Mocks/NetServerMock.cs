using System;
using System.Linq.Expressions;
using Moq;
using Moq.Protected;
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

        public void VerifyOnBeforeStart(Times times)
        {
            this.Protected().Verify("OnBeforeStart", times);
        }
        public void VerifyOnAfterStart(Times times)
        {
            this.Protected().Verify("OnAfterStart", times);
        }

        public void VerifyOnBeforeStop(Times times)
        {
            this.Protected().Verify("OnBeforeStop", times);
        }

        public void VerifyOnAfterStop(Times times)
        {
            this.Protected().Verify("OnAfterStop", times);
        }

        public void VerifyOnClientConnected(Times times)
        {
            this.Protected().Verify("OnClientConnected", times, ItExpr.IsAny<TClient>());
        }

        public void VerifyOnClientDisconnected(Times times)
        {
            this.Protected().Verify("OnClientDisconnected", times, ItExpr.IsAny<TClient>());
        }
    }
}
