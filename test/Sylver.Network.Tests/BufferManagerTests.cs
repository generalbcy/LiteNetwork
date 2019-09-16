using Sylver.Network.Common;
using System;
using System.Net.Sockets;
using Xunit;

namespace Sylver.Network.Tests
{
    public class BufferManagerTests : IDisposable
    {
        private readonly int _numberOfSlots = 50;
        private readonly int _bufferSize;
        private readonly int _totalBufferSize;
        private readonly BufferManager _bufferManager;

        public BufferManagerTests()
        {
            this._bufferSize = 256;
            this._totalBufferSize = this._bufferSize * this._numberOfSlots;
            this._bufferManager = new BufferManager(this._totalBufferSize, this._bufferSize);
        }

        [Fact]
        public void CreateBufferManagerTest()
        {
            Assert.NotNull(this._bufferManager);
            Assert.Equal(this._totalBufferSize, this._bufferManager.Size);
        }

        [Fact]
        public void GetBufferTest()
        {
            var socketAsync = new SocketAsyncEventArgs();

            Assert.Null(socketAsync.Buffer);
            bool setBufferResult = this._bufferManager.SetBuffer(socketAsync);

            Assert.True(setBufferResult);
            Assert.NotNull(socketAsync.Buffer);
            Assert.Equal(this._totalBufferSize, socketAsync.Buffer.Length);
            Assert.Equal(0, socketAsync.Offset);
            Assert.Equal(this._bufferSize, socketAsync.Count);
        }

        [Fact]
        public void FreeBufferTest()
        {
            var socketAsync = new SocketAsyncEventArgs();

            Assert.Null(socketAsync.Buffer);
            bool setBufferResult = this._bufferManager.SetBuffer(socketAsync);

            Assert.True(setBufferResult);
            Assert.NotNull(socketAsync.Buffer);
            Assert.Equal(this._totalBufferSize, socketAsync.Buffer.Length);
            Assert.Equal(0, socketAsync.Offset);
            Assert.Equal(this._bufferSize, socketAsync.Count);

            this._bufferManager.FreeBuffer(socketAsync);
            Assert.Null(socketAsync.Buffer);
            Assert.Equal(0, socketAsync.Offset);
            Assert.Equal(0, socketAsync.Count);
        }

        [Fact]
        public void DisposeBufferManagerTest()
        {
            Assert.Equal(this._totalBufferSize, this._bufferManager.Size);
            this._bufferManager.Dispose();
            Assert.Equal(0, this._bufferManager.Size);
        }

        [Fact]
        public void GetSetMultipleBuffersTest()
        {
            const int number = 20;
            var socketsEvents = new SocketAsyncEventArgs[number];

            for (int i = 0; i < number; i++)
            {
                socketsEvents[i] = new SocketAsyncEventArgs();
                var socketAsync = socketsEvents[i];

                Assert.Null(socketAsync.Buffer);

                bool setBufferResult = this._bufferManager.SetBuffer(socketAsync);

                Assert.True(setBufferResult);
                Assert.NotNull(socketAsync.Buffer);
                Assert.Equal(this._bufferSize * i, socketAsync.Offset);
            }

            for (int i = 0; i < 10; i++)
            {
                this._bufferManager.FreeBuffer(socketsEvents[i]);
            }

            for (int i = 0; i < 10; i++)
            {
                var socketAsync = socketsEvents[i];
                Assert.Null(socketAsync.Buffer);

                bool setBufferResult = this._bufferManager.SetBuffer(socketAsync);

                Assert.True(setBufferResult);
                Assert.NotNull(socketAsync.Buffer);
            }
        }

        [Fact]
        public void SetBufferWhenNoMoreSpaceAvailableTest()
        {
            var socketsEvents = new SocketAsyncEventArgs[this._numberOfSlots];

            for (int i = 0; i < this._numberOfSlots; i++)
            {
                socketsEvents[i] = new SocketAsyncEventArgs();
                var socketAsync = socketsEvents[i];

                Assert.Null(socketAsync.Buffer);

                bool setBufferResult = this._bufferManager.SetBuffer(socketAsync);

                Assert.True(setBufferResult);
                Assert.NotNull(socketAsync.Buffer);
                Assert.Equal(this._bufferSize * i, socketAsync.Offset);
            }

            var otherSocketAsync = new SocketAsyncEventArgs();
            bool result = this._bufferManager.SetBuffer(otherSocketAsync);

            Assert.False(result);
        }

        public void Dispose() => this._bufferManager.Dispose();
    }
}
