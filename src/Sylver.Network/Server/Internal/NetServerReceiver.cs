using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Data;
using System;
using System.Buffers;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Sylver.Network.Server.Internal
{
    internal sealed class NetServerReceiver<TClient>
        where TClient : class, INetServerClient
    {
        private readonly ObjectPool<SocketAsyncEventArgs> _readPool;
        private readonly NetServer<TClient> _server;
        private readonly NetPacketParser _packetParser;
        private readonly int _receiveBufferLength;

        /// <summary>
        /// Creates a new <see cref="NetServerReceiver{TClient}"/> instance.
        /// </summary>
        public NetServerReceiver(NetServer<TClient> server)
        {
            this._readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            this._server = server;
            this._receiveBufferLength = server.ServerConfiguration.ClientBufferSize;
            this._packetParser = new NetPacketParser(server.PacketProcessor);
        }

        /// <summary>
        /// Initialize the client and starts receving data.
        /// </summary>
        /// <param name="client">Client to initialize.</param>
        public void StartReceivingData(TClient client)
        {
            SocketAsyncEventArgs socketAsyncEvent = this.GetSocketEventFromPool();
            socketAsyncEvent.UserToken = new NetToken<TClient>(client);

            this.ReceiveData(client, socketAsyncEvent);
        }

        /// <summary>
        /// Receive data from a client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ReceiveData(TClient client, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (!client.Socket.ReceiveAsync(socketAsyncEvent))
            {
                this.ProcessReceive(socketAsyncEvent.UserToken as NetToken<TClient>, socketAsyncEvent);
            }
        }

        /// <summary>
        /// Process the received data.
        /// </summary>
        /// <param name="clientToken">Client token.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        private void ProcessReceive(NetToken<TClient> clientToken, SocketAsyncEventArgs socketAsyncEvent)
        {
            if (socketAsyncEvent == null)
            {
                throw new ArgumentNullException("Cannot receive data from a null socket event.", nameof(socketAsyncEvent));
            }

            if (socketAsyncEvent.BytesTransferred > 0)
            {
                if (socketAsyncEvent.SocketError == SocketError.Success)
                {
                    this._packetParser.ParseIncomingData(clientToken, socketAsyncEvent.Buffer, socketAsyncEvent.BytesTransferred);

                    if (clientToken.IsMessageComplete)
                    {
                        byte[] test = clientToken.HeaderData.Concat(clientToken.MessageData).ToArray();

                        string a = "[" + string.Join(", ", test) + "]";

                        var message = clientToken.MessageData.Skip(4).Take(clientToken.MessageSize.Value - 4).ToArray();

                        Console.WriteLine($"Received {clientToken.Client.Id}: '{Encoding.UTF8.GetString(message)}'");
                        // TODO: dispatch message
                        clientToken.Reset();
                    }

                    this.ReceiveData(clientToken.Client, socketAsyncEvent);
                }
                else
                {
                    Console.WriteLine($"Socket error: {socketAsyncEvent.SocketError}.");
                }
            }
            else
            {
                this.ReturnSocketEvent(socketAsyncEvent);
                this._server.DisconnectClient(clientToken.Client.Id);
            }
        }

        /// <summary>
        /// Fired when a receive operation has completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Socket async event arguments.</param>
        private void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                this.ProcessReceive(e.UserToken as NetToken<TClient>, e);
            }
            else
            {
                throw new InvalidOperationException($"Unknown '{e.LastOperation}' socket operation in receiver.");
            }
        }

        /// <summary>
        /// Gets a socket event from the event pool.
        /// </summary>
        /// <returns>Available <see cref="SocketAsyncEventArgs"/>.</returns>
        private SocketAsyncEventArgs GetSocketEventFromPool()
        {
            SocketAsyncEventArgs socketAsyncEvent = this._readPool.Get();

            socketAsyncEvent.SetBuffer(ArrayPool<byte>.Shared.Rent(this._receiveBufferLength), 0, this._receiveBufferLength);
            socketAsyncEvent.Completed += this.OnCompleted;

            return socketAsyncEvent;
        }

        /// <summary>
        /// Returns the socket event into the event pool.
        /// </summary>
        /// <param name="socketAsyncEvent">Socket async event to return.</param>
        private void ReturnSocketEvent(SocketAsyncEventArgs socketAsyncEvent)
        {
            ArrayPool<byte>.Shared.Return(socketAsyncEvent.Buffer, true);

            socketAsyncEvent.SetBuffer(null, 0, 0);
            socketAsyncEvent.UserToken = null;
            socketAsyncEvent.Completed -= this.OnCompleted;

            this._readPool.Return(socketAsyncEvent);
        }
    }
}
