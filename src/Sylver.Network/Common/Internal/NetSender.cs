using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Sylver.Network.Common.Internal
{
    internal abstract class NetSender : INetSender
    {
        private readonly BlockingCollection<NetMessageData> _sendingCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        private bool _disposedValue;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates and initializes a new <see cref="NetSender"/> base instance.
        /// </summary>
        protected NetSender()
        {
            this._sendingCollection = new BlockingCollection<NetMessageData>();
            this._cancellationTokenSource = new CancellationTokenSource();
            this._cancellationToken = this._cancellationTokenSource.Token;
        }

        /// <inheritdoc />
        public void Start()
        {
            Task.Factory.StartNew(this.ProcessSendingQueue,
                this._cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            this.IsRunning = true;
        }

        /// <inheritdoc />
        public void Stop()
        {
            this._cancellationTokenSource.Cancel(false);
            this.IsRunning = false;
        }

        /// <inheritdoc />
        public void Send(NetMessageData message) => this._sendingCollection.Add(message);

        /// <summary>
        /// Gets a <see cref="SocketAsyncEventArgs"/> for the sending operation.
        /// </summary>
        /// <returns></returns>
        protected abstract SocketAsyncEventArgs GetSocketEvent();

        /// <summary>
        /// Clears an used <see cref="SocketAsyncEventArgs"/>.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async vent arguments to clear.</param>
        protected abstract void ClearSocketEvent(SocketAsyncEventArgs socketAsyncEvent);

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
        /// Fired when a send operation has been completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        protected void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ClearSocketEvent(e);
        }

        /// <summary>
        /// Disposes the sender resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    this.Stop();
                    this._sendingCollection.Dispose();
                    this._cancellationTokenSource.Dispose();
                }

                this._disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="NetSender"/> resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
