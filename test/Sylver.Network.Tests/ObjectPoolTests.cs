using Moq;
using Sylver.Network.Common;
using System;
using Xunit;

namespace Sylver.Network.Tests
{
    public class ObjectPoolTests
    {
        private readonly ObjectPool<IDisposable> _pool;

        public ObjectPoolTests()
        {
            this._pool = new ObjectPool<IDisposable>();
        }

        [Fact]
        public void PushPopObjectInPool()
        {
            var obj = new Mock<IDisposable>();
            this._pool.Push(obj.Object);
            var poppedObject = this._pool.Pop();

            Assert.NotNull(poppedObject);
            Assert.Same(obj.Object, poppedObject);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public void CountObjectsInPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = new Mock<IDisposable>();
                this._pool.Push(obj.Object);
            }
            Assert.Equal(count, this._pool.Count);

            this._pool.Clear();
            Assert.Equal(0, this._pool.Count);
        }

        [Fact]
        public void PopEmptyPool()
        {
            var poppedObject = this._pool.Pop();

            Assert.Null(poppedObject);
        }

        [Fact]
        public void PushEmptyItem()
        {
            Assert.Throws<ArgumentNullException>(() => this._pool.Push(null));
        }

        [Fact]
        public void DisposePool()
        {
            var obj = new Mock<IDisposable>();
            this._pool.Push(obj.Object);

            Assert.Equal(1, this._pool.Count);
            this._pool.Dispose();
            Assert.Equal(0, this._pool.Count);
            obj.Verify(x => x.Dispose(), Times.Once());
        }
    }
}
