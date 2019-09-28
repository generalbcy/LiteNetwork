using Microsoft.Extensions.ObjectPool;
using Sylver.Network.Common;
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
        private readonly int _receiveBufferLength;

        /// <summary>
        /// Creates a new <see cref="NetServerReceiver{TClient}"/> instance.
        /// </summary>
        public NetServerReceiver(NetServer<TClient> server)
        {
            this._readPool = ObjectPool.Create<SocketAsyncEventArgs>();
            this._server = server;
            this._receiveBufferLength = 32;
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
                    this.ParseIncomingData(clientToken, socketAsyncEvent, this._server.PacketProcessor);

                    if (clientToken.IsMessageComplete)
                    {
                        var message = clientToken.MessageData.Skip(4).Take(clientToken.MessageSize.Value - 4).ToArray();

                        Console.WriteLine($"Received {clientToken.Client.Id}: '{Encoding.UTF8.GetString(message)}'");
                        // TODO: dispatch message
                        clientToken.ResetData();
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
                Console.WriteLine("Disconnected");
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

        /// <summary>
        /// Parse incoming data.
        /// </summary>
        /// <param name="token">Client token information.</param>
        /// <param name="socketAsyncEvent">Socket async event arguments.</param>
        /// <param name="packetProcessor">Server packet processor.</param>
        private void ParseIncomingData(NetToken<TClient> token, SocketAsyncEventArgs socketAsyncEvent, IPacketProcessor packetProcessor)
        {
            while (token.DataStartOffset < socketAsyncEvent.BytesTransferred)
            {
                int headerSize = packetProcessor.HeaderSize;

                if (token.ReceivedHeaderBytesCount < headerSize)
                {
                    if (token.HeaderData == null)
                        token.HeaderData = new byte[headerSize];

                    int bufferRemainingBytes = socketAsyncEvent.BytesTransferred - token.DataStartOffset;
                    int headerRemainingBytes = headerSize - token.ReceivedHeaderBytesCount;
                    int bytesToRead = Math.Min(bufferRemainingBytes, headerRemainingBytes);

                    Buffer.BlockCopy(socketAsyncEvent.Buffer, token.DataStartOffset, token.HeaderData, token.ReceivedHeaderBytesCount, bytesToRead);

                    token.ReceivedHeaderBytesCount += bytesToRead;
                    token.DataStartOffset += bytesToRead;
                }

                if (token.ReceivedHeaderBytesCount == headerSize && token.HeaderData != null)
                {
                    if (!token.MessageSize.HasValue)
                        token.MessageSize = packetProcessor.GetMessageLength(token.HeaderData);
                    if (token.MessageSize.Value < 0)
                        throw new InvalidOperationException("Message size cannot be smaller than zero.");

                    if (token.MessageData == null)
                        token.MessageData = new byte[token.MessageSize.Value];

                    if (token.ReceivedMessageBytesCount < token.MessageSize.Value)
                    {
                        int bufferRemainingBytes = socketAsyncEvent.BytesTransferred - token.DataStartOffset;
                        int messageRemainingBytes = token.MessageSize.Value - token.ReceivedMessageBytesCount;
                        int bytesToRead = Math.Min(bufferRemainingBytes, messageRemainingBytes);

                        Buffer.BlockCopy(socketAsyncEvent.Buffer, token.DataStartOffset, token.MessageData, token.ReceivedMessageBytesCount, bytesToRead);

                        token.ReceivedMessageBytesCount += bytesToRead;
                        token.DataStartOffset += bytesToRead;
                    }
                }
            }

            token.DataStartOffset = 0;
        }
    }
}
