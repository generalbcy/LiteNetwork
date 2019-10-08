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

        public NetServerSender()
        {
            this._writePool = ObjectPool.Create<SocketAsyncEventArgs>();
            this._sendingCollection = new BlockingCollection<NetMessageData>();
            this._cancellationTokenSource = new CancellationTokenSource();
            this._cancellationToken = this._cancellationTokenSource.Token;
        }

        public void Start()
        {
            Task.Factory.StartNew(this.ProcessSendingQueue, 
                this._cancellationToken, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Default);
        }

        public void Stop()
        {
            this._cancellationTokenSource.Cancel(false);
        }

        public void Dispose()
        {
            this.Stop();
        }

        private void ProcessSendingQueue()
        {
            while (true)
            {
                try
                {
                    NetMessageData message = this._sendingCollection.Take(this._cancellationToken);

                    if (message.Connection != null && message.Data != null)
                    {
                        this.SendMessage(message);
                    }
                }
                catch
                {
                    if (this._cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }

        private void SendMessage(NetMessageData message)
        {
            // TODO: send message
        }
    }
}
