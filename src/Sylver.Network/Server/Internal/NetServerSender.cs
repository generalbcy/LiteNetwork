using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Common;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerSender : IDisposable
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _writePool;
        private readonly BlockingCollection<NetMessageData> _sendingCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Gets a value that indiciates if the sender is running and processing packets.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates a new <see cref="NetServerSender"/> instance.
        /// </summary>
        public NetServerSender()
        {
            this._writePool = ObjectPool.Create<SocketAsyncEventArgs>();
            this._sendingCollection = new BlockingCollection<NetMessageData>();
            this._cancellationTokenSource = new CancellationTokenSource();
            this._cancellationToken = this._cancellationTokenSource.Token;
        }

        /// <summary>
        /// Starts the sender process queue.
        /// </summary>
        public void Start()
        {
            Task.Factory.StartNew(this.ProcessSendingQueue, 
                this._cancellationToken, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Default);
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops the sender process.
        /// </summary>
        public void Stop()
        {
            this._cancellationTokenSource.Cancel(false);
            this.IsRunning = false;
        }

        /// <summary>
        /// Enqueue a <see cref="NetMessageData"/> into the sender queue.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public void Send(NetMessageData message) => this._sendingCollection.Add(message);

        /// <summary>
        /// Stops and disposes the current sender.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            this._sendingCollection.Dispose();
            this._cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Dequeue the message collection and sends the messages to their recipients.
        /// </summary>
        private void ProcessSendingQueue()
        {
            while (!this._cancellationToken.IsCancellationRequested)
            {
                try
                {
                    NetMessageData message = this._sendingCollection.Take(this._cancellationToken);

                    if (message.Connection != null && message.Data != null)
                    {
                        this.SendMessage(message.Connection, message.Data);
                    }
                }
                catch (OperationCanceledException)
                {
                    // The operation has been cancelled: nothing to do
                }
            }
        }

        /// <summary>
        /// Sends the message data to the given <see cref="INetConnection"/>.
        /// </summary>
        /// <param name="connection">Client connection.</param>
        /// <param name="data">Message data.</param>
        private void SendMessage(INetConnection connection, byte[] data)
        {
            SocketAsyncEventArgs socketAsyncEvent = this.GetSocketEvent();

            socketAsyncEvent.SetBuffer(data, 0, data.Length);

            if (!connection.Socket.SendAsync(socketAsyncEvent))
            {
                this.OnSendCompleted(this, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> from the pool.
        /// </summary>
        /// <returns></returns>
        private SocketAsyncEventArgs GetSocketEvent()
        {
            SocketAsyncEventArgs socketAsyncEvent = this._writePool.Get();

            socketAsyncEvent.Completed += this.OnSendCompleted;

            return socketAsyncEvent;
        }

        /// <summary>
        /// Returns to the prool and clears a used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent"></param>
        private void ReturnSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            socketAsyncEvent.Completed -= this.OnSendCompleted;
            socketAsyncEvent.SetBuffer(null, 0, 0);

            this._writePool.Return(socketAsyncEvent);
        }

        /// <summary>
        /// Fired when a send operation has been completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ReturnSocketEvent(e);
        }
    }
}
