using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Web_Server;

namespace Console_Client
{
    public class Program
    {
        private static GrpcChannel _channel;
        private static Greeter.GreeterClient _client;
        private static Guid _guid;
        private static void Startup()
        {
            _channel = GrpcChannel.ForAddress("http://127.0.0.1:5000",
                new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Insecure
                });
            _client = new Greeter.GreeterClient(_channel);
        }
        public static async Task Main()
        {
            Console.Write("What is your name? ");
            Startup();
            string name = Console.ReadLine();
            var reply = await _client.SayHelloAsync(
                new HelloRequest { Name = name });

            if (!Guid.TryParse(reply.Guid, out _guid))
            {
                Console.WriteLine("Error - corrupted connection!\nExiting program now.");
                Environment.Exit(-1);
            };
            
            Console.WriteLine(reply.Message);
            do
            {
                Console.Write("You: ");
                var message = Console.ReadLine();
                if (message == ":q") { break; }
                _client.StartTalking(new HelloRequest { Guid = _guid.ToString(), Message = message });
            } while (true);

            _channel.ShutdownAsync().Wait();
        }
    }
}