namespace Sylver.Network.Data.Internal
{
    internal class NetDataToken
    {
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
        public bool IsMessageComplete => this.MessageSize.HasValue ? this.ReceivedMessageBytesCount == this.MessageSize.Value : false;

        /// <summary>
        /// Reset the token data properties.
        /// </summary>
        public void Reset()
        {
            this.ReceivedHeaderBytesCount = 0;
            this.ReceivedMessageBytesCount = 0;
            this.HeaderData = null;
            this.MessageData = null;
            this.MessageSize = null;
        }
    }
}
