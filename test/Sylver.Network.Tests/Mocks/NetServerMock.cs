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
            this.SetupProtectedMethods();
        }

        public NetServerMock(MockBehavior behavior)
            : base(behavior)
        {
            this.SetupProtectedMethods();
        }

        public NetServerMock(MockBehavior behavior, params object[] args)
            : base(behavior, args)
        {
            this.SetupProtectedMethods();
        }

        public NetServerMock(Expression<Func<NetServer<TClient>>> newExpression, MockBehavior behavior = MockBehavior.Loose)
            : base(newExpression, behavior)
        {
            this.SetupProtectedMethods();
        }

        public void VerifyOnStart(Times times)
        {
            this.Protected().Verify("OnStart", times);
        }

        public void VerifyOnStop(Times times)
        {
            this.Protected().Verify("OnStop", times);
        }

        public void VerifyOnClientConnected(Times times)
        {
            this.Protected().Verify("OnClientConnected", times, ItExpr.IsAny<TClient>());
        }

        public void VerifyOnClientDisconnected(Times times)
        {
            this.Protected().Verify("OnClientDisconnected", times, ItExpr.IsAny<TClient>());
        }

        private void SetupProtectedMethods()
        {
            this.Protected().Setup("OnStart").Verifiable();
            this.Protected().Setup("OnStop").Verifiable();
            this.Protected().Setup("OnClientConnected", ItExpr.IsNull<TClient>()).Verifiable();
            this.Protected().Setup("OnClientDisconnected", ItExpr.IsNull<TClient>()).Verifiable();
        }
    }
}
