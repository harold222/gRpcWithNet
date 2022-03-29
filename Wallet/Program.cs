using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;

namespace Wallet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var input = new HelloRequest { Name = "Harold" };
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true
            );

            var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new Greeter.GreeterClient(channel);

            var reply = await client.SayHelloAsync(input);
            Console.WriteLine($"Rspuesta: {reply.Message}");
            Console.ReadLine();
        }
    }
}
