using Sylver.Network.Common;
using Sylver.Network.Server;
using System;
using System.Net.Sockets;

namespace Sylver.TCPServer
{
    public class Client : NetConnection, INetServerClient
    {
    }

    public class Server : NetServer<Client>
    {
        
    }

    class Program
    {
        public static void Main()
        {
            var server = new Server();

            server.Start();

            Console.ReadKey();
        }
    }
}
