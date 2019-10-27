using Bogus;
using Sylver.Network.Data;
using Sylver.Network.Server;
using Sylver.Network.Tests.Server.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sylver.Network.Tests.Server
{
    public sealed class NetServerClientTests : IDisposable
    {
        private readonly Randomizer _randomizer;
        private readonly NetServerConfiguration _serverConfiguration;
        private readonly NetServerMock<CustomClient> _serverMock;
        private readonly CustomClient _customClient;
        private readonly CustomClient _otherClient;
        private readonly IEnumerable<CustomClient> _customClientsList;
        private readonly INetPacketStream _packet;

        public NetServerClientTests()
        {
            this._randomizer = new Randomizer((int)DateTime.UtcNow.Ticks);
            this._serverConfiguration = new NetServerConfiguration("127.0.0.1", 4444);
            this._serverMock = new NetServerMock<CustomClient>(this._serverConfiguration);
            this._customClient = new CustomClient(null)
            {
                Server = this._serverMock.Object
            };
            this._otherClient = new CustomClient(null);
            this._customClientsList = Enumerable.Repeat(new CustomClient(null), this._randomizer.Byte());
            this._packet = new NetPacket();
            this._packet.Write(this._randomizer.String(this._randomizer.Byte()));
        }

        [Fact]
        public void SendPacketToItSelftTest()
        {
            this._customClient.Send(this._packet);
        }

        [Fact]
        public void SendNullPacketToItSelfTest()
        {
            Assert.Throws<ArgumentNullException>(() => this._customClient.Send(null));
        }

        [Fact]
        public void SendPacketToOtherClientTest()
        {
            this._customClient.SendTo(this._otherClient, this._packet);
        }

        [Fact]
        public void SendNullPacketToOtherClientTest()
        {
            Assert.Throws<ArgumentNullException>(() => this._customClient.SendTo(this._otherClient, null));
        }

        [Fact]
        public void SendPacketToNullClient()
        {
            Assert.Throws<ArgumentNullException>(() => this._customClient.SendTo(client: null, this._packet));
        }

        [Fact]
        public void SendPacketToListOfClientsTest()
        {
            this._customClient.SendTo(this._customClientsList, this._packet);
        }

        [Fact]
        public void SendNullPacketToListOfClientsTest()
        {
            Assert.Throws<ArgumentNullException>(() => this._customClient.SendTo(this._customClientsList, null));
        }

        [Fact]
        public void SendPacketToAnUndefinedListOfClientsTest()
        {
            Assert.Throws<ArgumentNullException>(() => this._customClient.SendTo(clients: null, this._packet));
        }

        [Fact]
        public void SendPacketToAllTest()
        {
            this._customClient.SendToAll(this._packet);
        }

        [Fact]
        public void SendNullPacketToAllTest()
        {
            Assert.Throws<ArgumentNullException>(() => this._customClient.SendToAll(null));
        }

        public void Dispose()
        {
            this._packet.Dispose();
            this._customClient.Dispose();
            this._otherClient.Dispose();

            foreach (IDisposable disposableObject in this._customClientsList)
                disposableObject.Dispose();
        }
    }
}
