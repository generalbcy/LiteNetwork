using Sylver.Network.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sylver.Network.Tests")]
namespace Sylver.Network.Server
{
    public interface INetServerClient : INetConnection
    {
    }
}
