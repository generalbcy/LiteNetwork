using Sylver.Network.Data;
using Sylver.Network.Server;
using System;
using System.Net.Sockets;

namespace Sylver.TCPServer
{
    public class Client : NetServerClient
    {
        public Client(Socket socketConnection) 
            : base(socketConnection)
        {
        }

        public override void HandleMessage(INetPacketStream packetStream)
        {
            var message = packetStream.Read<string>();

            Console.WriteLine($"Received from client: '{message}'");

            this.SendBackToClient(message);
        }

        private void SendBackToClient(string message)
        {
            using (var packet = new NetPacket())
            {
                packet.Write($"Echo: '{message}'");

                this.SendPacket(packet);
            }
        }
    }

    public class Server : NetServer<Client>
    {
        public Server(NetServerConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void OnBeforeStart()
        {
            Console.WriteLine($"Loading stuff before starting the server...");
        }

        protected override void OnAfterStart()
        {
            Console.WriteLine("Server started!");
        }

        protected override void OnBeforeStop()
        {
            Console.WriteLine($"Clear and dispose stuff before shutting down the server.");
        }

        protected override void OnAfterStop()
        {
            Console.WriteLine("Server stopped!");
        }

        protected override void OnClientConnected(Client client)
        {
            Console.WriteLine($"New client with id '{client.Id}' connected.");
        }

        protected override void OnClientDisconnected(Client client)
        {
            Console.WriteLine($"Client with id '{client.Id}' disconnected.");
        }
    }

    class Program
    {
        public static void Main()
        {
            var configuration = new NetServerConfiguration("127.0.0.1", 4444);

            using (var server = new Server(configuration))
            {
                server.Start();

                Console.ReadKey();
            }
        }
    }
}
