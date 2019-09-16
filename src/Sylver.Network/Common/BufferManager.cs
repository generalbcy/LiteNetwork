using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Sylver.Network.Common
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up and 
    /// assigned to <see cref="SocketAsyncEventArgs"/> objects for use with each socket I/O operation.  
    /// This enables bufffers to be easily reused and guards against fragmenting heap memory.
    /// </summary>
    public class BufferManager : IDisposable
    {
        private readonly int _bufferSize;
        private readonly ConcurrentStack<int> _freeBufferIndexes;
        private byte[] _buffer;
        private int _currentIndex;

        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        public int Size => this._buffer != null ? this._buffer.Length : 0;

        /// <summary>
        /// Creates a new <see cref="BufferManager"/> instance.
        /// </summary>
        /// <param name="totalBytesToAllocate">Total amount of bytes to allocate.</param>
        public BufferManager(int totalBytesToAllocate, int bufferSize)
        {
            this._currentIndex = 0;
            this._bufferSize = bufferSize;
            this._buffer = new byte[totalBytesToAllocate];
            this._freeBufferIndexes = new ConcurrentStack<int>();
        }

        /// <summary>
        /// Assigns a buffer from the buffer pool to the specified <see cref="SocketAsyncEventArgs"/> object.
        /// </summary>
        /// <param name="args"><see cref="SocketAsyncEventArgs"/> object</param>
        /// <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (this._freeBufferIndexes.Count > 0)
            {
                if (this._freeBufferIndexes.TryPop(out int index))
                    args.SetBuffer(this._buffer, index, this._bufferSize);
            }
            else
            {
                if ((this.Size - this._bufferSize) < this._currentIndex)
                    return false;

                args.SetBuffer(this._buffer, this._currentIndex, this._bufferSize);
                this._currentIndex += this._bufferSize;
            }

            return true;
        }

        /// <summary>
        /// Removes the buffer from a <see cref="SocketAsyncEventArgs"/> object. This frees the buffer back to the buffer pool.
        /// </summary>
        /// <param name="args"><see cref="SocketAsyncEventArgs"/> object</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this._freeBufferIndexes.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        /// <summary>
        /// Dispose the <see cref="BufferManager"/> instance.
        /// </summary>
        public void Dispose()
        {
            this._freeBufferIndexes.Clear();
            this._buffer = null;
        }
    }
}
