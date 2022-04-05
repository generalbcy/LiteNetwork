﻿using LiteNetwork.Client;
using LiteNetwork.Protocol.Abstractions;

namespace LiteNetwork.Samples.Hosting.Server
{
    internal class Client : LiteClient
    {
        public Client(LiteClientOptions options, IServiceProvider serviceProvider = null!) 
            : base(options, serviceProvider)
        {
        }

        public override Task HandleMessageAsync(ILitePacketStream incomingPacketStream)
        {
            string message = incomingPacketStream.ReadString();

            Console.WriteLine($"Received from server: {message}");

            return Task.CompletedTask;
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Client connected.");
            base.OnConnected();
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected");
            base.OnDisconnected();
        }
    }
}
