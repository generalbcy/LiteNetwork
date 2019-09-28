using System;

namespace Sylver.Network.Common
{
    internal sealed class NetToken<TUser> : IDisposable
        where TUser : class, INetConnection
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        public TUser Client { get; private set; }

        /// <summary>
        /// Gets or sets the message header data.
        /// </summary>
        public byte[] HeaderData { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes received for the the message header.
        /// </summary>
        public int ReceivedHeaderBytesCount { get; set; }

        /// <summary>
        /// Gets or sets the full message size.
        /// </summary>
        public int? MessageSize { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes received for the message body.
        /// </summary>
        public int ReceivedMessageBytesCount { get; set; }

        /// <summary>
        /// Gets or sets the received message data.
        /// </summary>
        public byte[] MessageData { get; set; }

        /// <summary>
        /// Gets or sets the data start offset.
        /// </summary>
        public int DataStartOffset { get; set; }

        /// <summary>
        /// Gets a value that indicates if the message is complete.
        /// </summary>
        public bool IsMessageComplete => this.ReceivedMessageBytesCount == this.MessageSize.Value;

        /// <summary>
        /// Creates a new <see cref="NetToken"/> instance.
        /// </summary>
        /// <param name="client">Client instance.</param>
        public NetToken(TUser client)
        {
            this.Client = client;
        }

        /// <summary>
        /// Resest the token data properties.
        /// </summary>
        public void ResetData()
        {
            this.DataStartOffset = 0;
            this.ReceivedHeaderBytesCount = 0;
            this.ReceivedMessageBytesCount = 0;
            this.HeaderData = null;
            this.MessageData = null;
            this.MessageSize = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.ResetData();
            this.Client = null;
        }
    }
}
