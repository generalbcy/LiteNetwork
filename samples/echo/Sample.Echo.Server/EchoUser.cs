using LiteNetwork.Server;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LiteNetwork.Sample.Echo.Server
{
    public class EchoUser : LiteServerUser
    {
        public override Task HandleMessageAsync(byte[] packetBuffer)
        {
            using var incomingPacketStream = new MemoryStream(packetBuffer);
            using var packetReader = new BinaryReader(incomingPacketStream);

            string receivedMessage = packetReader.ReadString();

            Console.WriteLine($"Received from '{Id}': {receivedMessage}");

            using var outgoingPacketStream = new MemoryStream();
            using var packetWriter = new BinaryWriter(outgoingPacketStream);

            packetWriter.Write($"Received: '{receivedMessage}'.");

            Send(packetWriter.BaseStream);

            return Task.CompletedTask;
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"New client connected with id: {Id}");

            using var outgoingPacketStream = new MemoryStream();
            using var packetWriter = new BinaryWriter(outgoingPacketStream);
            packetWriter.Write($"Hello {Id}!");
            
            Send(packetWriter.BaseStream);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client '{Id}' disconnected.");
        }
    }
}
