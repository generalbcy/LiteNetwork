using Sylver.Network.Common;
using Sylver.Network.Server;
using System;
using System.Net.Sockets;

namespace Sylver.TCPServer
{
    public class Client : NetConnection, INetServerClient
    {
        public Client(Socket socketConnection) 
            : base(socketConnection)
        {
        }
    }

    public class Server : NetServer<Client>
    {
        public Server(NetServerConfiguration configuration)
            : base(configuration)
        {
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
