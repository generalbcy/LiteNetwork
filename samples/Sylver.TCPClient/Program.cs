using Sylver.Network.Client;
using Sylver.Network.Data;
using System;

namespace Sylver.TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var myClient = new MyClient();

            myClient.Connect();

            while (true)
            {
                string messageToSend = Console.ReadLine();

                if (messageToSend == "quit")
                {
                    break;
                }
                else
                {
                    myClient.SendMessage(messageToSend);   
                }
            }

            myClient.Dispose();
        }
    }

    public sealed class MyClient : NetClient
    {
        public MyClient()
        {
            var retry = new NetClientRetryConfiguration(NetClientRetryOption.Limited, 100);
            this.ClientConfiguration = new NetClientConfiguration("127.0.0.1", 4444, 128, retry);
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to server");
        }

        public override void HandleMessage(INetPacketStream packet)
        {
            string message = packet.Read<string>();

            Console.WriteLine($"Received from server: {message}");
        }

        public void SendMessage(string message)
        {
            using (INetPacketStream packet = new NetPacket())
            {
                packet.Write(message);

                this.SendMessage(packet);
            }
        }
    }
}
