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
            // get current enviroment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var input = new HelloRequest { Name = "Harold" };

            // protocol http unsafe
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true
            );

            string addressToConnect =
                environment == "development" ?
                    "http://blockchain:80" :
                    "http://localhost:5000";

            var channel = GrpcChannel.ForAddress(addressToConnect);
            var client = new Greeter.GreeterClient(channel);

            var reply = await client.SayHelloAsync(input);
            Console.WriteLine($"Rspuesta: {reply.Message}");
            Console.ReadLine();
        }
    }
}
