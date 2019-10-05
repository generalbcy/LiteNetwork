using System;

namespace Sylver.Network.Data
{
    public interface INetPacketStream : IDisposable
    {
        byte[] Buffer { get; }

        NetPacketStateType State { get; }

        T Read<T>();

        void Write<T>(T value);
    }
}
