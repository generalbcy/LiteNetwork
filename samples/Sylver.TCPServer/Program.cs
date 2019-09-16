using Sylver.Network.Common;
using Sylver.Network.Server;
using System;

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
            using (var server = new Server())
            {
                server.Start();

                Console.ReadKey();
            }
        }
    }
}
