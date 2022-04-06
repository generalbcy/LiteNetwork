using LiteNetwork.Client.Internal;
using LiteNetwork.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LiteNetwork.Client
{
    public class LiteClient : ILiteConnection
    {
        /// <summary>
        /// The event used when the client has been connected.
        /// </summary>
        public event EventHandler? Connected;

        /// <summary>
        /// The event used when the client has been disconnected.
        /// </summary>
        public event EventHandler? Disconnected;

        /// <summary>
        /// The event used when the client has encountered an error.
        /// </summary>
        public event EventHandler<Exception>? Error;

        private readonly IServiceProvider _serviceProvider = null!;
        private readonly ILogger<LiteClient>? _logger;
        private readonly LiteClientConnector _connector;
        private readonly LiteSender _sender;
        private readonly LiteClientReceiver _receiver;

        public Guid Id { get; }

        public Socket Socket { get; }

        public LiteClientOptions Options { get; }

        /// <summary>
        /// Creates a new <see cref="LiteClient"/> instance with the given <see cref="LiteClientOptions"/>.
        /// </summary>
        /// <param name="options">A client configuration options.</param>
        /// <param name="serviceProvider">Service provider to use.</param>
        public LiteClient(LiteClientOptions options, IServiceProvider serviceProvider = null!)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Id = Guid.NewGuid();
            Options = options;
            _serviceProvider = serviceProvider;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _connector = new LiteClientConnector(Socket, Options.Host, Options.Port);
            _sender = new LiteSender(this);
            _receiver = new LiteClientReceiver(options.PacketProcessor, options.ReceiveStrategy, options.BufferSize);
            _receiver.Error += (s, e) => OnError(e);

            if (_serviceProvider is not null)
            {
                _logger = _serviceProvider.GetService<ILogger<LiteClient>>();
            }
        }

        public virtual Task HandleMessageAsync(byte[] packetBuffer)
        {
            return Task.CompletedTask;
        }

        public virtual void Send(byte[] packetBuffer) => _sender.Send(packetBuffer);

        public async Task ConnectAsync()
        {
            _logger?.LogTrace($"Connecting to {Options.Host}:{Options.Port}.");
            bool isConnected = await _connector.ConnectAsync();

            if (isConnected)
            {
                _sender.Start();
                _receiver.StartReceiving(this);
                OnConnected();
                _logger?.LogTrace($"Connected to {Options.Host}:{Options.Port}.");
            }
        }

        public async Task DisconnectAsync()
        {
            _logger?.LogTrace($"Disconnecting from {Options.Host}:{Options.Port}.");
            bool isDisconnected = await _connector.DisconnectAsync();

            if (isDisconnected)
            {
                _sender.Stop();
                OnDisconnected();
                _logger?.LogTrace($"Disconnected from {Options.Host}:{Options.Port}.");
            }
        }

        /// <summary>
        /// Fired when the client has been connected.
        /// </summary>
        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, null);
        }

        /// <summary>
        /// Fired when the client has been disconnected.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, null);
        }

        /// <summary>
        /// Fired when the client has encountered an error.
        /// </summary>
        /// <param name="exception">Exception with the error.</param>
        protected virtual void OnError(Exception exception)
        {
            Error?.Invoke(this, exception);
        }

        /// <summary>
        /// Dispose the client resources.
        /// </summary>
        public void Dispose()
        {
            _connector.Dispose();
            _sender.Dispose();
            _receiver.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
