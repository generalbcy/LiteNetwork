# LiteNetwork

[![Build](https://github.com/Eastrall/LiteNetwork/actions/workflows/build.yml/badge.svg)](https://github.com/Eastrall/LiteNetwork/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/LiteNetwork.svg)](https://www.nuget.org/packages/LiteNetwork/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/LiteNetwork)](https://www.nuget.org/packages/LiteNetwork/)

`LiteNetwork` is a simple and fast networking library built with C# and compatible with .NET Standard 2, .NET 5 and .NET 6. Its main goal is to simply the creation of basic socket servers over the TCP/IP protocol.

Initially, LiteNetwork has been initialy developed for game development networking, but can also be used for other purposes.

## How to install

`LiteNetwork` is shiped as a single package, you can install it through the Visual Studio project package manager or using the following command in the Package Manager Console:

```sh
$> Install-Package LiteNetwork
```

Or you can use the dotnet command:

```sh
$> dotnet add package LiteNetwork
```

## Getting started

### Create a server

There is two ways of building a TCP server with `LiteNetwork`:
* The instance way, by creating a `LiteServer` instance and then run it manually
* The service way
    * In fact, `LiteNetwork` provides an extension to the `ServiceCollection` object, and can be integrated in a [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host) (used by ASP.NET Core, MAUI).

#### Common code

First of all, you will need to create the user class that will represent a connected user on your server. Simple create a new `class` that implements the `LiteServerUser` class.

```csharp
using LiteNetwork.Server;

public class ClientUser : LiteServerUser
{
}
```

Within this class, you will be able to handle this client's incoming message sent by a client program thanks to the `HandleMessageAsync()` method.
You can also be notified when the client connects to the server or disconnects.

```csharp
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server;

public class TcpUser : LiteServerUser
{
    public override Task HandleMessageAsync(byte[] packetBuffer)
    {
        // Handle incoming messages using a BinaryReader or any other solution for reading a byte[].
    }

    protected override void OnConnected()
    {
        // When the client connects.
    }

    protected override void OnDisconnected()
    {
        // When the client disconnects.
    }
}
```

Once the server user is ready, you can create the server itself that will handle this `TcpUser` type of users.
Create another new `class`, and implement the `LiteServer<T>` class where `T` is the previously created `TcpUser`.

```csharp
public class MyTcpServer : LiteServer<TcpUser>
{
    public MyTcpServer(LiteServerOptions options, IServiceProvider serviceProvider = null)
        : base(options, serviceProvider)
    {
    }
}
```
The server has some hooks that allows you to control its life time, such as:

| Method | Description |
|--------|-------------|
| `OnBeforeStart()` | Called before the server starts. |
| `OnAfterStart()` | Called after the server starts.  |
| `OnBeforeStop()` | Called before the server stops. |
| `OnAfterStop()` | Called after the server stops. |
| `OnError(ILiteConnection, Exception)` | Called when there is an unhandled error witht the given `ILiteConnection`. |


#### Create the server via instance

Now that the server and user classes are built, you can now instanciate your server and call the `Start()` method to start the server.

```csharp
// Using minimal API
using LiteNetwork.Server;
using System;

// Create the server configuration, to listen on "127.0.0.1" and port "4444"
var configuration = new LiteServerOptions()
{
    Host = "127.0.0.1",
    Port = 4444
};

// Create the server instance by givin the server options and start it.
using var server = new MyTcpServer(configuration);
server.Start();

// Just for the example, otherwise the console will just shutdown.
// Do not use in production environment.
Console.ReadKey(); 
```

#### Create the server via service

For this example, you will need to install the [`Microsoft.Extensions.Hosting`](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/) package from nuget in order to build a [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host).

```csharp
// Using minimal API
using Microsoft.Extensions.Hosting;
using System;

var host = new HostBuilder()
    .UseConsoleLifetime()
    .Build();

return host.RunAsync();
```

Then, once your host is setup and running, you can configure the `LiteServer` service using the `ConfigureLiteNetwork()` method located in the `LiteNetwork.Hosting` namespace:

```csharp
// Using minimal API
using LiteNetwork.Hosting;
using LiteNetwork.Server.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

var host = new HostBuilder()
    // Configures the LiteNetwork context.
    .ConfigureLiteNetwork((context, builder) =>
    {
        // Adds a LiteServer instance for the MyTcpServer class.
        builder.AddLiteServer<MyTcpServer>(options =>
        {
            // This configures the server's LiteServerOptions instance.
            options.Host = "127.0.0.1";
            options.Port = 4444;
        });
    })
    .UseConsoleLifetime()
    .Build();

return host.RunAsync();
```

Your server is now listening on "127.0.0.1" and port "4444".
Also, since you are using a .NET generic host, it also provides dependency injection into the server and client classes. Hence, you can inject services, configuration (`IOptions<T>` if configured, etc..).

> Note: You can also add as many servers you want into a single .NET generic host by calling the `builder.AddLiteServer<>()` method with different parameters.

### Create a client

TBA.

## Protocol

### Packet Processor

TBA.

## Thanks

I would like to thank everyone that contributed to this library directly by fixing bugs or add new features, but also the people with who I had the chance to discuss about networking problematics which helped me to improve this library.

## Credits

Package Icon : from [Icons8](https://icons8.com/)
