using System;
using System.Collections.Concurrent;

namespace Sylver.Network.Common
{
    /// <summary>
    /// Provides a mecanism to handle a pool of objects.
    /// </summary>
    /// <typeparam name="T">Object type that implements <see cref="IDisposable"/>.</typeparam>
    public class ObjectPool<T> : IDisposable where T : class, IDisposable
    {
        private readonly ConcurrentStack<T> _pool;

        /// <summary>
        /// Gets the number of elements available in the pool.
        /// </summary>
        public int Count => this._pool.Count;

        /// <summary>
        /// Creates a new <see cref="ObjectPool{T}"/> instance.
        /// </summary>
        public ObjectPool()
        {
            this._pool = new ConcurrentStack<T>();
        }

        /// <summary>
        /// Push a new item to the pool.
        /// </summary>
        /// <param name="item">Item to push.</param>
        public void Push(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this._pool.Push(item);
        }

        /// <summary>
        /// Try to pop the object on the top of the pool. Returs null if pool is empty.
        /// </summary>
        /// <returns>Object on top of the pool. Null if pool is empty.</returns>
        public T Pop() => this._pool.TryPop(out T result) ? result : null;

        /// <summary>
        /// Clear and dispose all elements of the pool.
        /// </summary>
        public void Clear()
        {
            while (!this._pool.IsEmpty)
            {
                if (this._pool.TryPop(out T result))
                    result.Dispose();
            }
        }

        /// <summary>
        /// Dispose the pool elements.
        /// </summary>
        public void Dispose() => this.Clear();
    }
}
