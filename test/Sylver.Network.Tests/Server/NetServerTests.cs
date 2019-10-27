using Moq;
using Sylver.Network.Common;
using Sylver.Network.Data;
using Sylver.Network.Server;
using Sylver.Network.Tests.Mocks;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerTests
    {
        private readonly NetSocketMock _socketMock;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _server;

        public NetServerTests()
        {
            this._socketMock = new NetSocketMock();
            this._socketMock.ConfigureAcceptResult(true);

            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);

            this._server = new NetServerMock<CustomClient>(this._serverConfiguration);
            this._server.SetupGet(x => x.Socket).Returns(this._socketMock);
        }

        [Fact]
        public void StartServerTest()
        {
            this._server.Object.Start();

            Assert.True(this._server.Object.IsRunning);
            Assert.Equal(this._serverConfiguration.Host, this._server.Object.ServerConfiguration.Host);
            Assert.Equal(this._serverConfiguration.Port, this._server.Object.ServerConfiguration.Port);
            Assert.Equal(this._serverConfiguration.Backlog, this._server.Object.ServerConfiguration.Backlog);
            Assert.Equal(this._serverConfiguration.ClientBufferSize, this._server.Object.ServerConfiguration.ClientBufferSize);

            this._socketMock.VerifySetSocketOptions(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this._socketMock.VerifyBind(NetHelper.CreateIpEndPoint(this._serverConfiguration.Host, this._serverConfiguration.Port));
            this._socketMock.VerifyListen(this._serverConfiguration.Backlog);
            this._server.VerifyOnBeforeStart(Times.Once());
            this._server.VerifyOnAfterStart(Times.Once());
            Assert.True(this._server.BeforeStartCalled);
            Assert.True(this._server.AfterStartCalled);
        }

        [Fact]
        public void StartServerTwiceTest()
        {
            this._server.Object.Start();

            Assert.True(this._server.Object.IsRunning);
            Assert.Throws<InvalidOperationException>(() => this._server.Object.Start());
        }

        [Fact]
        public void StopServerTest()
        {
            this._server.Object.Start();
            Assert.True(this._server.Object.IsRunning);
            this._server.Object.Stop();
            Assert.False(this._server.Object.IsRunning);
            this._socketMock.VerifyDispose();
            this._server.VerifyOnBeforeStop(Times.Once());
            this._server.VerifyOnAfterStop(Times.Once());
            Assert.True(this._server.BeforeStopCalled);
            Assert.True(this._server.AfterStopCalled);
        }

        [Fact]
        public void ChangePacketProcessorBeforeStartTest()
        {
            Assert.NotNull(this._server.Object.PacketProcessor);
            Assert.IsType<NetPacketProcessor>(this._server.Object.PacketProcessor);
            this._server.Object.PacketProcessor = new CustomNetPacketProcessor(false);

            Assert.IsType<CustomNetPacketProcessor>(this._server.Object.PacketProcessor);

            this._server.Object.Start();

            Assert.Equal(sizeof(long), this._server.Object.PacketProcessor.HeaderSize);
            Assert.False(this._server.Object.PacketProcessor.IncludeHeader);
        }

        [Fact]
        public void ChangePacketProcessorAfterStartTest()
        {
            Assert.NotNull(this._server.Object.PacketProcessor);
            Assert.IsType<NetPacketProcessor>(this._server.Object.PacketProcessor);

            this._server.Object.Start();

            Assert.Throws<InvalidOperationException>(() => this._server.Object.PacketProcessor = new CustomNetPacketProcessor(false));
        }
    }
}
