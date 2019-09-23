namespace Sylver.Network.Common
{
    internal sealed class NetToken<TUser> where TUser : INetConnection
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        public TUser Client { get; }

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
            this.ReceivedHeaderBytesCount = 0;
            this.ReceivedMessageBytesCount = 0;
            this.HeaderData = null;
            this.MessageData = null;
            this.MessageSize = null;
        }
    }
}
