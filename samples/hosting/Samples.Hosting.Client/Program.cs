using LiteNetwork.Client.Hosting;
using LiteNetwork.Hosting;
using Microsoft.Extensions.Hosting;

namespace LiteNetwork.Samples.Hosting.Server
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.Title = "LiteNetwork Hosting Sample (Client)";

            var host = new HostBuilder()
                .ConfigureLiteNetwork((context, builder) =>
                {
                    builder.AddLiteClient<Client>(options =>
                    {
                        options.Host = "127.0.0.1";
                        options.Port = 4444;
                    });
                })
                .UseConsoleLifetime()
                .Build();

            return host.RunAsync();
        }
    }
}
