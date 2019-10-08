namespace Sylver.Network.Common
{
    internal sealed class NetMessageData
    {
        public INetConnection Connection { get; }

        public byte[] Data { get; }

        public NetMessageData(INetConnection connection, byte[] data)
        {
            this.Connection = connection;
            this.Data = data;
        }
    }
}
