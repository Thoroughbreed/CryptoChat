using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chat;
using Grpc.Core;
using Channel = Grpc.Core.Channel;

namespace Console_Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Welcome! Please type in your username: ");
            var userName = Console.ReadLine();
            // Include port of the gRPC server as an application argument
            var port = args.Length > 0 ? args[0] : "5000";

            // var channel = GrpcChannel.ForAddress("http://127.0.0.1:" + port,
            //     new GrpcChannelOptions
            //     {
            //         Credentials = ChannelCredentials.Insecure
            //     });
            Console.Write("Please type in IP of the server as xxx.xxx.xxx.xxx: ");
            var ip = Console.ReadLine();
            
            var channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);
            
            var client = new ChatRoom.ChatRoomClient(channel);

            using (var chat = client.join())
            {
                _ = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                    {
                        var response = chat.ResponseStream.Current;
                        Console.WriteLine($"{response.User}: {response.Text}");
                    }
                });

                await chat.RequestStream.WriteAsync(new Message { User = userName, Text = $"{userName} has joined the room" });

                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToLower() == "bye")
                    {
                        break;
                    }
                    await chat.RequestStream.WriteAsync(new Message { User = userName, Text = line });
                }
                await chat.RequestStream.CompleteAsync();
            }

            Console.WriteLine("Disconnecting");
            await channel.ShutdownAsync();
        }
    }
}