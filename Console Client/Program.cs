using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Web_Server;

namespace Console_Client
{
    public class Program
    {
        private static GrpcChannel channel;
        private static Greeter.GreeterClient client;
        private static void Startup()
        {
            channel = GrpcChannel.ForAddress("http://127.0.0.1:5000",
                new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Insecure
                });
            client = new Greeter.GreeterClient(channel);
        }
        public static async Task Main()
        {
            Console.Write("What is your name? ");
            Startup();
            string name = Console.ReadLine();
            var reply = await client.SayHelloAsync(
                new HelloRequest { Name = name });
            Console.WriteLine(reply.Message);

            do
            {
                Console.Write("You: ");
                var message = Console.ReadLine();
                if (message == ":q") { break; }
                client.StartTalking(new HelloRequest { Name = name, Message = message });
            } while (true);

            channel.ShutdownAsync().Wait();
        }
    }
}