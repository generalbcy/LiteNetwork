using Sylver.Network.Common;
using Sylver.Network.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sylver.Network.Client
{
    public class NetClient : INetClient
    {
        private bool _disposedValue;
        private IPacketProcessor _packetProcessor;

        public Guid Id { get; }

        public INetSocket Socket { get; }

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public IPacketProcessor PacketProcessor
        {
            get => this._packetProcessor;
            set
            {
                if (this.IsRunning)
                    throw new InvalidOperationException("Cannot update packet processor when server is running.");
                this._packetProcessor = value;
            }
        }

        protected NetClient()
        {
            this.Id = Guid.NewGuid();
            this._packetProcessor = new NetPacketProcessor();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose the <see cref="NetClient"/> resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Connect()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual void HandleMessage(INetPacketStream packet)
        {
            // Nothing to do. Must be override in child classes.
        }

        /// <inheritdoc />
        public void SendMessage(INetPacketStream packet)
        {
            throw new NotImplementedException();
        }
    }
}
