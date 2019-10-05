using System;

namespace Sylver.Network.Data
{
    public interface INetPacketStream : IDisposable
    {
        T Read<T>();

        void Write<T>(T value);
    }
}
